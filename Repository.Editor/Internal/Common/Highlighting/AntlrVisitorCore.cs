using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Repository.Common.Validation;
using Repository.Editor.Highlighting;

namespace Repository.Editor.Internal.Common.Highlighting
{
    internal class AntlrVisitorCore
    {
        private readonly IParseTreeVisitor<Task> _visitor;
        private readonly ITextColorer _colorer;
        private readonly IAntlrSyntaxProvider _syntaxProvider;
        private readonly CommonTokenStream _tokenStream;
        private readonly CancellationToken _cancellationToken;

        private int _tokenIndex;
        private ParserRuleContext _lastAncestor;
        private ReadOnlyList<SyntaxReplacement> _replacements;

        public AntlrVisitorCore(
            IParseTreeVisitor<Task> visitor,
            ParserRuleContext tree,
            ITextColorer colorer,
            IAntlrSyntaxProvider syntaxProvider,
            CommonTokenStream tokenStream,
            CancellationToken cancellationToken)
        {
            Verify.NotNull(visitor, nameof(visitor));
            Verify.NotNull(tree, nameof(tree));
            Verify.NotNull(colorer, nameof(colorer));
            Verify.NotNull(syntaxProvider, nameof(syntaxProvider));
            Verify.NotNull(tokenStream, nameof(tokenStream));

            _visitor = visitor;
            _colorer = colorer;
            _syntaxProvider = syntaxProvider;
            _tokenStream = tokenStream;
            _cancellationToken = cancellationToken;
            _lastAncestor = tree;
            _replacements = ReadOnlyList<SyntaxReplacement>.Empty;
        }

        // Even if AbstractParseTreeVisitor.AggregateResult() is overridden, the default implementation
        // of AbstractParseTreeVisitor.VisitChildren() only starts Task objects, it doesn't await them.
        // It's necessary to override the method in each visitor to await the child tasks.
        public async Task VisitChildren(IRuleNode node)
        {
            int childCount = node.ChildCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = node.GetChild(i);
                await child.Accept(_visitor);
            }
        }

        public Task VisitChildren(ParserRuleContext context, SyntaxReplacement additionalReplacement)
        {
            var newReplacements = GetApplicableReplacements(context);
            newReplacements.Add(additionalReplacement);
            return VisitChildrenWithReplacements(context, newReplacements.AsReadOnlyList());
        }

        public Task VisitChildren(ParserRuleContext context, ImmutableArray<SyntaxReplacement> additionalReplacements)
        {
            var newReplacements = GetApplicableReplacements(context);
            newReplacements.AddRange(additionalReplacements);
            return VisitChildrenWithReplacements(context, newReplacements.AsReadOnlyList());
        }

        public Task VisitErrorNode(IErrorNode node)
            => node.Symbol.IsInvalid() ? Task.CompletedTask : VisitTerminal(node);

        public Task VisitTerminal(ITerminalNode node)
        {
            var replacementKind = FindTerminalReplacement(node).Kind;
            return Advance(node, replacementKind);
        }

        private async Task Advance(IToken token, SyntaxKind kind)
        {
            await Approach(token);
            await Surpass(token, kind);
        }

        private Task Advance(ITerminalNode node, SyntaxKind replacementKind)
        {
            var token = node.Symbol;
            var kind = _syntaxProvider.SuggestKind(token).TryReplace(replacementKind);
            return Advance(token, kind);
        }

        private async Task Approach(IToken token)
        {
            int start = _tokenIndex;
            int end = token.TokenIndex;

            for (int i = start; i < end; i++)
            {
                await Surpass(_tokenStream.Get(i));
            }
        }

        private SyntaxReplacement FindTerminalReplacement(ITerminalNode node)
        {
            var nodePath = NodePath.GetRelativePath(_lastAncestor, node);
            // The last replacements are the ones that were added most recently, by closer ancestors. Give precedence to those.
            for (int i = _replacements.Count - 1; i >= 0; i--)
            {
                var replacement = _replacements[i];
                if (nodePath.Equals(replacement.Path))
                {
                    return replacement;
                }
            }

            return SyntaxReplacement.None;
        }

        private List<SyntaxReplacement> GetApplicableReplacements(IParseTree tree)
        {
            var list = new List<SyntaxReplacement>();
            var treePath = NodePath.GetRelativePath(_lastAncestor, tree);

            foreach (var replacement in _replacements)
            {
                var path = replacement.Path;
                if (path.TryChangeRoot(treePath, out var newPath))
                {
                    list.Add(replacement.WithPath(newPath));
                }
            }

            return list;
        }

        private Task Surpass(IToken token)
            => Surpass(token, _syntaxProvider.GetKind(token));

        private Task Surpass(IToken token, SyntaxKind kind)
        {
            Verify.ValidState(_tokenIndex == token.TokenIndex, "We skipped some tokens!");

            if (kind == SyntaxKind.Eof)
            {
                return Task.CompletedTask;
            }

            _tokenIndex++;

            int count = token.Text.Length;
            return _colorer.Color(kind, count, _cancellationToken);
        }

        private async Task VisitChildrenWithReplacements(ParserRuleContext context, ReadOnlyList<SyntaxReplacement> replacements)
        {
            int childCount = context.ChildCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = context.GetChild(i);
                await VisitWithReplacements(child, context, replacements);
            }
        }

        private async Task VisitWithReplacements(IParseTree child, ParserRuleContext parent, ReadOnlyList<SyntaxReplacement> replacements)
        {
            Debug.Assert(child.Parent == parent);

            var originalAncestor = _lastAncestor;
            var originalReplacements = _replacements;

            _lastAncestor = parent;
            _replacements = replacements;
            await child.Accept(_visitor);

            _lastAncestor = originalAncestor;
            _replacements = originalReplacements;
        }
    }
}

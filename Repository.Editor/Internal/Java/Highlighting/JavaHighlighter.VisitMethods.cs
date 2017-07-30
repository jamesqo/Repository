using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Repository.Editor.Highlighting;
using Repository.Editor.Internal.Common.Highlighting;
using static Repository.Editor.Internal.Java.Highlighting.JavaParser;

namespace Repository.Editor.Internal.Java.Highlighting
{
    internal partial class JavaHighlighter
    {
        private partial class Visitor
        {
            private static SyntaxReplacement AnnotationNameReplacement { get; } =
                SyntaxReplacement.Create(NodePath.Create(typeof(QualifiedNameContext), typeof(TerminalNodeImpl)), SyntaxKind.Annotation);

            private static SyntaxReplacement AnnotationTypeDeclarationReplacement { get; } =
                SyntaxReplacement.Terminal(SyntaxKind.TypeDeclaration);

            private static SyntaxReplacement ClassDeclarationReplacement { get; } =
                SyntaxReplacement.Terminal(SyntaxKind.TypeDeclaration);

            private static SyntaxReplacement ClassOrInterfaceTypeReplacement { get; } =
                SyntaxReplacement.Terminal(SyntaxKind.TypeIdentifier);

            private static SyntaxReplacement ConstructorDeclarationReplacement { get; } =
                SyntaxReplacement.Terminal(SyntaxKind.ConstructorDeclaration);

            private static SyntaxReplacement CreatedNameReplacement { get; } =
                SyntaxReplacement.Terminal(SyntaxKind.TypeIdentifier);

            private static SyntaxReplacement EnumDeclarationReplacement { get; } =
                SyntaxReplacement.Terminal(SyntaxKind.TypeDeclaration);

            private static SyntaxReplacement FormalParameterReplacement { get; } =
                SyntaxReplacement.Create(NodePath.Create(typeof(VariableDeclaratorIdContext), typeof(TerminalNodeImpl)), SyntaxKind.ParameterDeclaration);

            private static SyntaxReplacement InterfaceDeclarationReplacement { get; } =
                SyntaxReplacement.Terminal(SyntaxKind.TypeDeclaration);

            private static SyntaxReplacement LastFormalParameterReplacement { get; } =
                SyntaxReplacement.Create(NodePath.Create(typeof(VariableDeclaratorIdContext), typeof(TerminalNodeImpl)), SyntaxKind.ParameterDeclaration);

            private static SyntaxReplacement MethodDeclarationReplacement { get; } =
                SyntaxReplacement.Terminal(SyntaxKind.MethodDeclaration);

            private static ImmutableArray<SyntaxReplacement> MethodInvocationReplacements { get; } =
                ImmutableArray.Create(
                    SyntaxReplacement.Create(NodePath.Create(typeof(PrimaryExpressionContext), typeof(PrimaryContext), typeof(TerminalNodeImpl)), SyntaxKind.MethodIdentifier),
                    SyntaxReplacement.Create(NodePath.Create(typeof(MemberAccessContext), typeof(TerminalNodeImpl)), SyntaxKind.MethodIdentifier));

            private static SyntaxReplacement NonWildcardTypeArgumentsReplacement { get; } =
                SyntaxReplacement.Terminal(SyntaxKind.Plaintext);

            private static SyntaxReplacement NonWildcardTypeArgumentsOrDiamondReplacement { get; } =
                SyntaxReplacement.Terminal(SyntaxKind.Plaintext);

            private static SyntaxReplacement TypeArgumentsReplacement { get; } =
                SyntaxReplacement.Terminal(SyntaxKind.Plaintext);

            private static SyntaxReplacement TypeArgumentsOrDiamondReplacement { get; } =
                SyntaxReplacement.Terminal(SyntaxKind.Plaintext);

            private static SyntaxReplacement TypeParameterReplacement { get; } =
                SyntaxReplacement.Terminal(SyntaxKind.TypeIdentifier);

            private static SyntaxReplacement TypeParametersReplacement { get; } =
                SyntaxReplacement.Terminal(SyntaxKind.Plaintext);

            private static SyntaxReplacement WildcardTypeArgumentReplacement { get; } =
                SyntaxReplacement.Terminal(SyntaxKind.TypeIdentifier);

            public override Task VisitAnnotationName([NotNull] AnnotationNameContext context)
                => VisitChildren(context, AnnotationNameReplacement);

            // TODO: Do annotation type declarations, which use '@interface', need special treatment?
            public override Task VisitAnnotationTypeDeclaration([NotNull] AnnotationTypeDeclarationContext context)
                => VisitChildren(context, AnnotationTypeDeclarationReplacement);

            public override Task VisitClassDeclaration([NotNull] ClassDeclarationContext context)
                => VisitChildren(context, ClassDeclarationReplacement);

            public override Task VisitClassOrInterfaceType([NotNull] ClassOrInterfaceTypeContext context)
                => VisitChildren(context, ClassOrInterfaceTypeReplacement);

            public override Task VisitConstructorDeclaration([NotNull] ConstructorDeclarationContext context)
                => VisitChildren(context, ConstructorDeclarationReplacement);

            public override Task VisitCreatedName([NotNull] CreatedNameContext context)
                => VisitChildren(context, CreatedNameReplacement);

            public override Task VisitEnumDeclaration([NotNull] EnumDeclarationContext context)
                => VisitChildren(context, EnumDeclarationReplacement);

            public override Task VisitErrorNode(IErrorNode node)
            {
                throw new NotImplementedException();
            }

            public override Task VisitFormalParameter([NotNull] FormalParameterContext context)
                => VisitChildren(context, FormalParameterReplacement);

            public override Task VisitInterfaceDeclaration([NotNull] InterfaceDeclarationContext context)
                => VisitChildren(context, InterfaceDeclarationReplacement);

            public override Task VisitLastFormalParameter([NotNull] LastFormalParameterContext context)
                => VisitChildren(context, LastFormalParameterReplacement);

            public override Task VisitMethodDeclaration([NotNull] MethodDeclarationContext context)
                => VisitChildren(context, MethodDeclarationReplacement);

            public override Task VisitMethodInvocation([NotNull] MethodInvocationContext context)
                => VisitChildren(context, MethodInvocationReplacements);

            public override Task VisitNonWildcardTypeArguments([NotNull] NonWildcardTypeArgumentsContext context)
                => VisitChildren(context, NonWildcardTypeArgumentsReplacement);

            public override Task VisitNonWildcardTypeArgumentsOrDiamond([NotNull] NonWildcardTypeArgumentsOrDiamondContext context)
                => VisitChildren(context, NonWildcardTypeArgumentsOrDiamondReplacement);

            public override Task VisitTerminal(ITerminalNode node)
            {
                var replacementKind = FindTerminalReplacement(node).Kind;
                return Advance(node, replacementKind);
            }

            public override Task VisitTypeArguments([NotNull] TypeArgumentsContext context)
                => VisitChildren(context, TypeArgumentsReplacement);

            public override Task VisitTypeArgumentsOrDiamond([NotNull] TypeArgumentsOrDiamondContext context)
                => VisitChildren(context, TypeArgumentsOrDiamondReplacement);

            public override Task VisitTypeParameter([NotNull] TypeParameterContext context)
                => VisitChildren(context, TypeParameterReplacement);

            public override Task VisitTypeParameters([NotNull] TypeParametersContext context)
                => VisitChildren(context, TypeParametersReplacement);

            public override Task VisitWildcardTypeArgument([NotNull] WildcardTypeArgumentContext context)
                => VisitChildren(context, WildcardTypeArgumentReplacement);

            // Even if AggregateResult() is overridden, the default implementation of VisitChildren()
            // can only start Task objects, not await them. It's necessary to override the method
            // ourselves to await each child task.
            public override async Task VisitChildren(IRuleNode node)
            {
                int childCount = node.ChildCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = node.GetChild(i);
                    await child.Accept(this);
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

            private Task VisitChildren(ParserRuleContext context, SyntaxReplacement additionalReplacement)
            {
                var newReplacements = GetApplicableReplacements(context);
                newReplacements.Add(additionalReplacement);
                return VisitChildrenWithReplacements(context, newReplacements.AsReadOnlyList());
            }

            private Task VisitChildren(ParserRuleContext context, ImmutableArray<SyntaxReplacement> additionalReplacements)
            {
                var newReplacements = GetApplicableReplacements(context);
                newReplacements.AddRange(additionalReplacements);
                return VisitChildrenWithReplacements(context, newReplacements.AsReadOnlyList());
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
                await child.Accept(this);

                _lastAncestor = originalAncestor;
                _replacements = originalReplacements;
            }
        }
    }
}
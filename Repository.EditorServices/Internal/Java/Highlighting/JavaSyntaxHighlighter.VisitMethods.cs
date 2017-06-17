using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Repository.EditorServices.Highlighting;
using Repository.EditorServices.Internal.Common.Highlighting;
using static Repository.EditorServices.Internal.Java.Highlighting.JavaParser;

namespace Repository.EditorServices.Internal.Java.Highlighting
{
    internal partial class JavaSyntaxHighlighter
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

            public override object VisitAnnotationName([NotNull] AnnotationNameContext context)
                => VisitChildren(context, AnnotationNameReplacement);

            // TODO: Do annotation type declarations, which use '@interface', need special treatment?
            public override object VisitAnnotationTypeDeclaration([NotNull] AnnotationTypeDeclarationContext context)
                => VisitChildren(context, AnnotationTypeDeclarationReplacement);

            public override object VisitClassDeclaration([NotNull] ClassDeclarationContext context)
                => VisitChildren(context, ClassDeclarationReplacement);

            public override object VisitClassOrInterfaceType([NotNull] ClassOrInterfaceTypeContext context)
                => VisitChildren(context, ClassOrInterfaceTypeReplacement);

            public override object VisitConstructorDeclaration([NotNull] ConstructorDeclarationContext context)
                => VisitChildren(context, ConstructorDeclarationReplacement);

            public override object VisitCreatedName([NotNull] CreatedNameContext context)
                => VisitChildren(context, CreatedNameReplacement);

            public override object VisitEnumDeclaration([NotNull] EnumDeclarationContext context)
                => VisitChildren(context, EnumDeclarationReplacement);

            public override object VisitFormalParameter([NotNull] FormalParameterContext context)
                => VisitChildren(context, FormalParameterReplacement);

            public override object VisitInterfaceDeclaration([NotNull] InterfaceDeclarationContext context)
                => VisitChildren(context, InterfaceDeclarationReplacement);

            public override object VisitLastFormalParameter([NotNull] LastFormalParameterContext context)
                => VisitChildren(context, LastFormalParameterReplacement);

            public override object VisitMethodDeclaration([NotNull] MethodDeclarationContext context)
                => VisitChildren(context, MethodDeclarationReplacement);

            public override object VisitMethodInvocation([NotNull] MethodInvocationContext context)
                => VisitChildren(context, MethodInvocationReplacements);

            public override object VisitNonWildcardTypeArguments([NotNull] NonWildcardTypeArgumentsContext context)
                => VisitChildren(context, NonWildcardTypeArgumentsReplacement);

            public override object VisitNonWildcardTypeArgumentsOrDiamond([NotNull] NonWildcardTypeArgumentsOrDiamondContext context)
                => VisitChildren(context, NonWildcardTypeArgumentsOrDiamondReplacement);

            public override object VisitTerminal(ITerminalNode node)
            {
                var replacementKind = FindTerminalReplacement(node).Kind;
                Advance(node, replacementKind);
                return null;
            }

            public override object VisitTypeArguments([NotNull] TypeArgumentsContext context)
                => VisitChildren(context, TypeArgumentsReplacement);

            public override object VisitTypeArgumentsOrDiamond([NotNull] TypeArgumentsOrDiamondContext context)
                => VisitChildren(context, TypeArgumentsOrDiamondReplacement);

            public override object VisitTypeParameter([NotNull] TypeParameterContext context)
                => VisitChildren(context, TypeParameterReplacement);

            public override object VisitTypeParameters([NotNull] TypeParametersContext context)
                => VisitChildren(context, TypeParametersReplacement);

            public override object VisitWildcardTypeArgument([NotNull] WildcardTypeArgumentContext context)
                => VisitChildren(context, WildcardTypeArgumentReplacement);

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

            private object VisitChildren(ParserRuleContext context, SyntaxReplacement additionalReplacement)
            {
                var newReplacements = GetApplicableReplacements(context);
                newReplacements.Add(additionalReplacement);
                return VisitChildrenWithReplacements(context, newReplacements.AsReadOnlyList());
            }

            private object VisitChildren(ParserRuleContext context, ImmutableArray<SyntaxReplacement> additionalReplacements)
            {
                var newReplacements = GetApplicableReplacements(context);
                newReplacements.AddRange(additionalReplacements);
                return VisitChildrenWithReplacements(context, newReplacements.AsReadOnlyList());
            }

            private object VisitChildrenWithReplacements(ParserRuleContext context, ReadOnlyList<SyntaxReplacement> replacements)
            {
                int childCount = context.ChildCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = context.GetChild(i);
                    VisitWithReplacements(child, context, replacements);
                }

                return null;
            }

            private void VisitWithReplacements(IParseTree child, ParserRuleContext parent, ReadOnlyList<SyntaxReplacement> replacements)
            {
                Debug.Assert(child.Parent == parent);

                var originalAncestor = _lastAncestor;
                var originalReplacements = _replacements;

                _lastAncestor = parent;
                _replacements = replacements;
                Visit(child);

                _lastAncestor = originalAncestor;
                _replacements = originalReplacements;
            }
        }
    }
}
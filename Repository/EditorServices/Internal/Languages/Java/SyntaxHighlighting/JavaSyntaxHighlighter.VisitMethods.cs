using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Repository.EditorServices.SyntaxHighlighting;
using static Repository.EditorServices.Internal.Languages.Java.SyntaxHighlighting.JavaParser;

namespace Repository.EditorServices.Internal.Languages.Java.SyntaxHighlighting
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

            private static SyntaxReplacement InterfaceDeclarationReplacement { get; } =
                SyntaxReplacement.Terminal(SyntaxKind.TypeDeclaration);

            private static SyntaxReplacement MethodDeclarationReplacement { get; } =
                SyntaxReplacement.Terminal(SyntaxKind.MethodDeclaration);

            private static ImmutableArray<SyntaxReplacement> MethodInvocationReplacements { get; } =
                ImmutableArray.Create(
                    SyntaxReplacement.Create(NodePath.Create(typeof(PrimaryExpressionContext), typeof(PrimaryContext), typeof(TerminalNodeImpl)), SyntaxKind.MethodIdentifier),
                    SyntaxReplacement.Create(NodePath.Create(typeof(MemberAccessContext), typeof(TerminalNodeImpl)), SyntaxKind.MethodIdentifier));

            private static SyntaxReplacement TypeParameterReplacement { get; } =
                SyntaxReplacement.Terminal(SyntaxKind.TypeIdentifier);

            private static SyntaxReplacement VariableDeclaratorIdReplacement { get; } =
                SyntaxReplacement.Terminal(SyntaxKind.ParameterDeclaration);

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

            public override object VisitInterfaceDeclaration([NotNull] InterfaceDeclarationContext context)
                => VisitChildren(context, InterfaceDeclarationReplacement);

            public override object VisitMethodDeclaration([NotNull] MethodDeclarationContext context)
                => VisitChildren(context, MethodDeclarationReplacement);

            public override object VisitMethodInvocation([NotNull] MethodInvocationContext context)
                => VisitChildren(context, MethodInvocationReplacements);

            public override object VisitTerminal(ITerminalNode node)
            {
                var replacementKind = FindTerminalReplacement(node).Kind;
                Advance(node, replacementKind);
                return null;
            }

            public override object VisitTypeParameter([NotNull] TypeParameterContext context)
                => VisitChildren(context, TypeParameterReplacement);

            // TODO: Affects declarators in try-with-resources and enhanced for loop, not only parameters.
            public override object VisitVariableDeclaratorId([NotNull] VariableDeclaratorIdContext context)
                => VisitChildren(context, VariableDeclaratorIdReplacement);

            private SyntaxReplacement FindTerminalReplacement(ITerminalNode node)
            {
                var nodePath = NodePath.GetRelativePath(_lastAncestor, node);
                // The last replacements are the ones that were added most recently, by closer ancestors. Give precedence to those.
                for (int i = _replacements.Length - 1; i >= 0; i--)
                {
                    var replacement = _replacements[i];
                    if (nodePath.Equals(replacement.Path))
                    {
                        return replacement;
                    }
                }

                return SyntaxReplacement.None;
            }

            private ImmutableArray<SyntaxReplacement>.Builder GetApplicableReplacements(IParseTree tree)
            {
                var treePath = NodePath.GetRelativePath(_lastAncestor, tree);
                var builder = ImmutableArray.CreateBuilder<SyntaxReplacement>();

                foreach (var replacement in _replacements)
                {
                    var path = replacement.Path;
                    if (path.TryChangeRoot(treePath, out var newPath))
                    {
                        builder.Add(replacement.WithPath(newPath));
                    }
                }

                return builder;
            }

            private object VisitChildren(ParserRuleContext context, SyntaxReplacement additionalReplacement)
            {
                var newReplacements = GetApplicableReplacements(context);
                newReplacements.Add(additionalReplacement);
                return VisitChildrenWithReplacements(context, newReplacements.ToImmutable());
            }

            private object VisitChildren(ParserRuleContext context, ImmutableArray<SyntaxReplacement> additionalReplacements)
            {
                var newReplacements = GetApplicableReplacements(context);
                newReplacements.AddRange(additionalReplacements);
                return VisitChildrenWithReplacements(context, newReplacements.ToImmutable());
            }

            private object VisitChildrenWithReplacements(ParserRuleContext context, ImmutableArray<SyntaxReplacement> replacements)
            {
                int childCount = context.ChildCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = context.GetChild(i);
                    VisitWithReplacements(child, context, replacements);
                }

                return null;
            }

            private void VisitWithReplacements(IParseTree child, ParserRuleContext parent, ImmutableArray<SyntaxReplacement> replacements)
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
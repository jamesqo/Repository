using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Repository.Editor.Highlighting;
using Repository.Editor.Internal.Common.Highlighting;
using static Repository.Editor.Internal.Java.Highlighting.JavaParser;

namespace Repository.Editor.Internal.Java.Highlighting
{
    internal partial class JavaHighlighter : IHighlighter
    {
        private class Visitor : JavaBaseVisitor<Task>
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

            private readonly AntlrVisitorCore _core;
            private readonly CompilationUnitContext _tree;

            internal Visitor(string text, ITextColorer colorer, CancellationToken cancellationToken)
            {
                var stream = AntlrUtilities.TokenStream(text, input => new JavaLexer(input));
                _tree = new JavaParser(stream).compilationUnit();
                _core = new AntlrVisitorCore(this, _tree, colorer, JavaSyntaxProvider.Instance, stream, cancellationToken);
            }

            internal Task Run() => _tree.Accept(this);

            public override Task VisitAnnotationName([NotNull] AnnotationNameContext context)
                => _core.VisitChildren(context, AnnotationNameReplacement);

            public override Task VisitAnnotationTypeDeclaration([NotNull] AnnotationTypeDeclarationContext context)
                => _core.VisitChildren(context, AnnotationTypeDeclarationReplacement);

            public override Task VisitClassDeclaration([NotNull] ClassDeclarationContext context)
                => _core.VisitChildren(context, ClassDeclarationReplacement);

            public override Task VisitClassOrInterfaceType([NotNull] ClassOrInterfaceTypeContext context)
                => _core.VisitChildren(context, ClassOrInterfaceTypeReplacement);

            public override Task VisitConstructorDeclaration([NotNull] ConstructorDeclarationContext context)
                => _core.VisitChildren(context, ConstructorDeclarationReplacement);

            public override Task VisitCreatedName([NotNull] CreatedNameContext context)
                => _core.VisitChildren(context, CreatedNameReplacement);

            public override Task VisitEnumDeclaration([NotNull] EnumDeclarationContext context)
                => _core.VisitChildren(context, EnumDeclarationReplacement);

            public override Task VisitErrorNode(IErrorNode node)
                => _core.VisitErrorNode(node);

            public override Task VisitFormalParameter([NotNull] FormalParameterContext context)
                => _core.VisitChildren(context, FormalParameterReplacement);

            public override Task VisitInterfaceDeclaration([NotNull] InterfaceDeclarationContext context)
                => _core.VisitChildren(context, InterfaceDeclarationReplacement);

            public override Task VisitLastFormalParameter([NotNull] LastFormalParameterContext context)
                => _core.VisitChildren(context, LastFormalParameterReplacement);

            public override Task VisitMethodDeclaration([NotNull] MethodDeclarationContext context)
                => _core.VisitChildren(context, MethodDeclarationReplacement);

            public override Task VisitMethodInvocation([NotNull] MethodInvocationContext context)
                => _core.VisitChildren(context, MethodInvocationReplacements);

            public override Task VisitNonWildcardTypeArguments([NotNull] NonWildcardTypeArgumentsContext context)
                => _core.VisitChildren(context, NonWildcardTypeArgumentsReplacement);

            public override Task VisitNonWildcardTypeArgumentsOrDiamond([NotNull] NonWildcardTypeArgumentsOrDiamondContext context)
                => _core.VisitChildren(context, NonWildcardTypeArgumentsOrDiamondReplacement);

            public override Task VisitTerminal(ITerminalNode node)
                => _core.VisitTerminal(node);

            public override Task VisitTypeArguments([NotNull] TypeArgumentsContext context)
                => _core.VisitChildren(context, TypeArgumentsReplacement);

            public override Task VisitTypeArgumentsOrDiamond([NotNull] TypeArgumentsOrDiamondContext context)
                => _core.VisitChildren(context, TypeArgumentsOrDiamondReplacement);

            public override Task VisitTypeParameter([NotNull] TypeParameterContext context)
                => _core.VisitChildren(context, TypeParameterReplacement);

            public override Task VisitTypeParameters([NotNull] TypeParametersContext context)
                => _core.VisitChildren(context, TypeParametersReplacement);

            public override Task VisitWildcardTypeArgument([NotNull] WildcardTypeArgumentContext context)
                => _core.VisitChildren(context, WildcardTypeArgumentReplacement);

            public override Task VisitChildren(IRuleNode node)
                => _core.VisitChildren(node);
        }

        public Task Highlight(string text, ITextColorer colorer, CancellationToken cancellationToken)
        {
            return new Visitor(text, colorer, cancellationToken).Run();
        }
    }
}

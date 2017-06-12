using System;
using System.Collections.Generic;
using System.Diagnostics;
using Android.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Repository.EditorServices.SyntaxHighlighting;
using Repository.Internal.EditorServices.SyntaxHighlighting.Grammars;
using static Repository.Internal.EditorServices.SyntaxHighlighting.Grammars.JavaParser;

namespace Repository.Internal.EditorServices.SyntaxHighlighting
{
    internal class JavaSyntaxHighlighter : ISyntaxHighlighter
    {
        private class Visitor : JavaBaseVisitor<object>
        {
            private readonly SpannableString _text;
            private readonly ISyntaxStyler _styler;
            private readonly CommonTokenStream _stream;

            private int _index;
            private int _lastTokenIndex;
            private SyntaxKind _kindOverride;

            internal Visitor(string text, ISyntaxStyler styler)
            {
                _text = new SpannableString(text);
                _styler = styler;
                _stream = AntlrUtilities.TokenStream(text, input => new JavaLexer(input));
                _lastTokenIndex = -1;
                _kindOverride = SyntaxKind.None;
            }

            public override object VisitAnnotationName([NotNull] AnnotationNameContext context)
                => VisitChildren(context, TargetedKindOverride.Create<QualifiedNameContext>(SyntaxKind.Annotation));

            // TODO: Do annotation type declarations, which use '@interface', need special treatment?
            public override object VisitAnnotationTypeDeclaration([NotNull] AnnotationTypeDeclarationContext context)
                => VisitChildren(context, TargetedKindOverride.Create<ITerminalNode>(SyntaxKind.TypeDeclaration));

            public override object VisitClassDeclaration([NotNull] ClassDeclarationContext context)
                => VisitChildren(context, TargetedKindOverride.Create<ITerminalNode>(SyntaxKind.TypeDeclaration));

            public override object VisitClassOrInterfaceType([NotNull] ClassOrInterfaceTypeContext context)
                => VisitChildren(context, TargetedKindOverride.Create<ITerminalNode>(SyntaxKind.TypeIdentifier));

            public override object VisitConstructorDeclaration([NotNull] ConstructorDeclarationContext context)
                => VisitChildren(context, TargetedKindOverride.Create<ITerminalNode>(SyntaxKind.ConstructorDeclaration));

            public override object VisitCreatedName([NotNull] CreatedNameContext context)
                => VisitChildren(context, TargetedKindOverride.Create<ITerminalNode>(SyntaxKind.TypeIdentifier));

            public override object VisitEnumDeclaration([NotNull] EnumDeclarationContext context)
                => VisitChildren(context, TargetedKindOverride.Create<ITerminalNode>(SyntaxKind.TypeDeclaration));

            public override object VisitInterfaceDeclaration([NotNull] InterfaceDeclarationContext context)
                => VisitChildren(context, TargetedKindOverride.Create<ITerminalNode>(SyntaxKind.TypeDeclaration));

            public override object VisitMethodDeclaration([NotNull] MethodDeclarationContext context)
                => VisitChildren(context, TargetedKindOverride.Create<ITerminalNode>(SyntaxKind.MethodDeclaration));

            public override object VisitTerminal(ITerminalNode node)
            {
                Advance(node, _kindOverride);
                return null;
            }

            public override object VisitTypeParameter([NotNull] TypeParameterContext context)
                => VisitChildren(context, TargetedKindOverride.Create<ITerminalNode>(SyntaxKind.TypeIdentifier));

            internal SpannableString HighlightText()
            {
                Visit(CreateTree());
                return _text;
            }

            private void Advance(IToken token, SyntaxKind kind)
            {
                Approach(token);
                Surpass(token, kind);
            }

            private void Advance(ITerminalNode node, SyntaxKind kindOverride)
            {
                var token = node.Symbol;
                var kind = SuggestKind(token).TryOverride(kindOverride);
                Advance(token, kind);
            }

            private void Approach(IToken token)
            {
                int startTokenIndex = _lastTokenIndex + 1;
                int endTokenIndex = token.TokenIndex;

                for (int i = startTokenIndex; i < endTokenIndex; i++)
                {
                    SurpassHidden(_stream.Get(i));
                }
            }

            private CompilationUnitContext CreateTree() => new JavaParser(_stream).compilationUnit();

            private SyntaxKind GetHiddenKind(IToken token)
            {
                switch (token.Type)
                {
                    case WS:
                        // TODO
                        return SyntaxKind.Identifier;
                    case COMMENT:
                    case LINE_COMMENT:
                        return SyntaxKind.Comment;
                    default:
                        throw new NotSupportedException();
                }
            }

            // TODO: Consider moving into partial class.
            private SyntaxKindSuggestion SuggestKind(IToken token)
            {
                // TODO: Better SyntaxKinds for certain types.
                switch (token.Type)
                {
                    case AntlrConstants.Eof:
                        return SyntaxKind.Eof;
                    case ABSTRACT:
                    case ASSERT:
                    case BOOLEAN:
                    case BREAK:
                    case BYTE:
                    case CASE:
                    case CATCH:
                    case CHAR:
                    case CLASS:
                    case CONST:
                    case CONTINUE:
                    case DEFAULT:
                    case DO:
                    case DOUBLE:
                    case ELSE:
                    case ENUM:
                    case EXTENDS:
                    case FINAL:
                    case FINALLY:
                    case FLOAT:
                    case FOR:
                    case IF:
                    case GOTO:
                    case IMPLEMENTS:
                    case IMPORT:
                    case INSTANCEOF:
                    case INT:
                    case INTERFACE:
                    case LONG:
                    case NATIVE:
                    case NEW:
                    case PACKAGE:
                    case PRIVATE:
                    case PROTECTED:
                    case PUBLIC:
                    case RETURN:
                    case SHORT:
                    case STATIC:
                    case STRICTFP:
                    case SUPER:
                    case SWITCH:
                    case SYNCHRONIZED:
                    case THIS:
                    case THROW:
                    case THROWS:
                    case TRANSIENT:
                    case TRY:
                    case VOID:
                    case VOLATILE:
                    case WHILE:
                        return SyntaxKind.Keyword;
                    case IntegerLiteral:
                    case FloatingPointLiteral:
                        return SyntaxKind.NumericLiteral;
                    case BooleanLiteral:
                        return SyntaxKind.BooleanLiteral;
                    case CharacterLiteral:
                    case StringLiteral:
                        return SyntaxKind.StringLiteral;
                    case NullLiteral:
                        return SyntaxKind.NullLiteral;
                    case LPAREN:
                    case RPAREN:
                        return SyntaxKind.Parenthesis;
                    case LBRACE:
                    case RBRACE:
                    case LBRACK:
                    case RBRACK:
                    case SEMI:
                    case COMMA:
                    case DOT:
                    case ASSIGN:
                    case GT:
                    case LT:
                    case BANG:
                    case TILDE:
                    case QUESTION:
                    case COLON:
                    case EQUAL:
                    case LE:
                    case GE:
                    case NOTEQUAL:
                    case AND:
                    case OR:
                    case INC:
                    case DEC:
                    case ADD:
                    case SUB:
                    case MUL:
                    case DIV:
                    case BITAND:
                    case BITOR:
                    case CARET:
                    case MOD:
                    case ADD_ASSIGN:
                    case SUB_ASSIGN:
                    case MUL_ASSIGN:
                    case DIV_ASSIGN:
                    case AND_ASSIGN:
                    case OR_ASSIGN:
                    case XOR_ASSIGN:
                    case MOD_ASSIGN:
                    case LSHIFT_ASSIGN:
                    case RSHIFT_ASSIGN:
                    case URSHIFT_ASSIGN:
                        return SyntaxKind.Identifier;
                    case Identifier:
                        return SyntaxKindSuggestion.Overridable(SyntaxKind.Identifier);
                    case AT:
                        return SyntaxKind.Annotation;
                    case ELLIPSIS:
                        return SyntaxKind.Identifier;
                    // Hidden token types are intentionally not handled here. GetHiddenKind() takes care of those.
                    default:
                        throw new NotSupportedException();
                }
            }

            private void Surpass(IToken token, SyntaxKind kind)
            {
                Debug.Assert(_lastTokenIndex + 1 == token.TokenIndex);

                if (kind == SyntaxKind.Eof)
                {
                    return;
                }

                _lastTokenIndex++;

                int count = token.Text.Length;
                var span = _styler.GetSpan(kind);
                _text.SetSpan(span, _index, _index + count, SpanTypes.InclusiveExclusive);
                _index += count;
            }

            private void SurpassHidden(IToken token)
            {
                var kind = GetHiddenKind(token);
                Surpass(token, kind);
            }

            private object VisitChildren(ParserRuleContext context, TargetedKindOverride kindOverride)
            {
                int childCount = context.ChildCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = context.GetChild(i);
                    if (child.GetType() == kindOverride.TargetType)
                    {
                        VisitWithKindOverride(child, kindOverride.Kind);
                    }
                    else
                    {
                        Visit(child);
                    }
                }

                return null;
            }

            private void VisitWithKindOverride(IParseTree tree, SyntaxKind kindOverride)
            {
                var original = _kindOverride;
                _kindOverride = kindOverride;
                Visit(tree);
                _kindOverride = original;
            }
        }

        public SpannableString Highlight(string text, ISyntaxStyler styler) => new Visitor(text, styler).HighlightText();
    }
}
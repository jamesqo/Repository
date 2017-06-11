using System;
using System.Collections.Generic;
using System.Diagnostics;
using Android.Text;
using Antlr4.Runtime;
using Repository.Internal.SyntaxHighlighting.Grammars;
using static Repository.Internal.SyntaxHighlighting.Grammars.JavaLexer;

namespace Repository.Internal.SyntaxHighlighting
{
    internal class JavaSyntaxHighlighter : ISyntaxHighlighter
    {
        private class CoreHighlighter
        {
            private readonly string _rawText;
            private readonly SpannableString _text;
            private readonly ISyntaxStyler _styler;

            private int _index;

            internal CoreHighlighter(string text, ISyntaxStyler styler)
            {
                _rawText = text;
                _text = new SpannableString(text);
                _styler = styler;
            }

            public void HighlightAll(IList<IToken> tokens)
            {
                int tokenCount = tokens.Count;

                for (int i = 0; i < tokenCount; i++)
                {
                    var token = tokens[i];
                    var kind = GetSyntaxKind(token);
                    Highlight(token, kind);

                    HandleAnnotationIdentifier(tokens, ref i);
                }
            }

            private void HandleAnnotationIdentifier(IList<IToken> tokens, ref int index)
            {
                if (tokens[index].Type == AT)
                {
                    var nextToken = tokens[++index];
                    Debug.Assert(nextToken.Type == Identifier);
                    Highlight(nextToken, SyntaxKind.Annotation);
                }
            }

            public void Highlight(IToken token, SyntaxKind tokenKind)
            {
                int count = token.Text.Length;
                // TODO: Set up handlers for Debug.Assert so this will fire.
                Debug.Assert(_rawText.Substring(_index, count) == token.Text);

                if (token.Type != AntlrUtilities.Eof)
                {
                    _text.SetSpan(_styler.GetSpan(tokenKind), _index, _index + count, SpanTypes.InclusiveExclusive);
                    _index += count;
                }
            }

            public SpannableString HighlightText()
            {
                var tokens = AntlrUtilities.Tokenize(_rawText, input => new JavaLexer(input));
                HighlightAll(tokens);
                return _text;
            }

            // TODO: More operators should be highlighted.
            private static SyntaxKind GetSyntaxKind(IToken token)
            {
                switch (token.Type)
                {
                    case AntlrUtilities.Eof:
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
                        return SyntaxKind.Keyword;
                    case CharacterLiteral:
                    case StringLiteral:
                        return SyntaxKind.StringLiteral;
                    case NullLiteral:
                        return SyntaxKind.Keyword;
                    case LPAREN:
                    case RPAREN:
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
                        return SyntaxKind.Identifier;
                    case AT:
                        return SyntaxKind.Annotation;
                    case ELLIPSIS:
                    case WS:
                        return SyntaxKind.Identifier;
                    case COMMENT:
                    case LINE_COMMENT:
                        return SyntaxKind.Comment;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public SpannableString Highlight(string text, ISyntaxStyler styler) => new CoreHighlighter(text, styler).HighlightText();
    }
}
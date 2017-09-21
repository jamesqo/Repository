using System;
using Antlr4.Runtime;
using Repository.Editor.Highlighting;
using Repository.Editor.Internal.Common.Highlighting;
using static Repository.Editor.Internal.Java.Highlighting.JavaParser;

namespace Repository.Editor.Internal.Java.Highlighting
{
    internal sealed class JavaSyntaxProvider : IAntlrSyntaxProvider
    {
        public static JavaSyntaxProvider Instance { get; } = new JavaSyntaxProvider();

        private JavaSyntaxProvider()
        {
        }

        public SyntaxSuggestion SuggestKind(IToken token)
        {
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
                case LBRACE:
                case RBRACE:
                case LBRACK:
                case RBRACK:
                case SEMI:
                case COMMA:
                case DOT:
                    return SyntaxKind.Plaintext;
                case ASSIGN:
                    return SyntaxKind.Operator;
                case GT:
                case LT:
                    // These tokens should not be considered operators in generic types or methods.
                    return SyntaxSuggestion.Replaceable(SyntaxKind.Operator);
                case BANG:
                case TILDE:
                    return SyntaxKind.Operator;
                case QUESTION:
                    // ? is not an operator in '? extends ...' in generics.
                    return SyntaxSuggestion.Replaceable(SyntaxKind.Operator);
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
                    return SyntaxKind.Operator;
                case Identifier:
                    return SyntaxSuggestion.Replaceable(SyntaxKind.Identifier);
                case AT:
                    return SyntaxKind.Annotation;
                case ELLIPSIS:
                    return SyntaxKind.Plaintext;
                case WS:
                    return SyntaxKind.Plaintext;
                case COMMENT:
                case LINE_COMMENT:
                    return SyntaxKind.Comment;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}

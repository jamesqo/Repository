using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Repository.EditorServices.Highlighting;
using Repository.EditorServices.Internal.Common.Highlighting;
using static Repository.EditorServices.Internal.Java.Highlighting.JavaParser;

namespace Repository.EditorServices.Internal.Java.Highlighting
{
    internal partial class JavaSyntaxHighlighter : ISyntaxHighlighter
    {
        private partial class Visitor : JavaBaseVisitor<object>
        {
            private readonly ISyntaxColorer _colorer;
            private readonly CommonTokenStream _stream;
            private readonly CompilationUnitContext _tree;

            private int _tokenIndex;
            private ParserRuleContext _lastAncestor;
            private ReadOnlyList<SyntaxReplacement> _replacements;

            internal Visitor(string text, ISyntaxColorer colorer)
            {
                _colorer = colorer;
                _stream = AntlrUtilities.TokenStream(text, input => new JavaLexer(input));
                _tree = new JavaParser(_stream).compilationUnit();
                _lastAncestor = _tree;
                _replacements = ReadOnlyList<SyntaxReplacement>.Empty;
            }

            internal void Run() => Visit(_tree);

            private void Advance(IToken token, SyntaxKind kind)
            {
                Approach(token);
                Surpass(token, kind);
            }

            private void Advance(ITerminalNode node, SyntaxKind replacementKind)
            {
                var token = node.Symbol;
                var kind = SuggestKind(token).TryReplace(replacementKind);
                Advance(token, kind);
            }

            private void Approach(IToken token)
            {
                int start = _tokenIndex;
                int end = token.TokenIndex;

                for (int i = start; i < end; i++)
                {
                    SurpassHidden(_stream.Get(i));
                }
            }

            private SyntaxKind GetHiddenKind(IToken token)
            {
                switch (token.Type)
                {
                    case WS:
                        return SyntaxKind.Plaintext;
                    case COMMENT:
                    case LINE_COMMENT:
                        return SyntaxKind.Comment;
                    default:
                        throw new NotSupportedException();
                }
            }

            private SyntaxSuggestion SuggestKind(IToken token)
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
                        // TODO: We should highlight this.
                        return SyntaxKind.Plaintext;
                    // Hidden token types are intentionally not handled here. GetHiddenKind() takes care of those.
                    default:
                        throw new NotSupportedException();
                }
            }

            private void Surpass(IToken token, SyntaxKind kind)
            {
                Debug.Assert(_tokenIndex == token.TokenIndex);

                if (kind == SyntaxKind.Eof)
                {
                    return;
                }

                _tokenIndex++;

                int count = token.Text.Length;
                _colorer.Color(kind, count);
            }

            private void SurpassHidden(IToken token)
            {
                var kind = GetHiddenKind(token);
                Surpass(token, kind);
            }
        }

        public TResult Highlight<TResult>(string text, ISyntaxColorer<TResult> colorer)
        {
            new Visitor(text, colorer).Run();
            return colorer.Result;
        }
    }
}
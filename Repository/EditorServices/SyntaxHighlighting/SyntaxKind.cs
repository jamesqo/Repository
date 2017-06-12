namespace Repository.EditorServices.SyntaxHighlighting
{
    // TODO: Introduce SyntaxKind.Plaintext or Generic or None or Default
    // TODO: As for the above, probably nah. SyntaxKind.Class(Declaration), SyntaxKind.Member(Declaration/Invocation)?
    // TODO: SyntaxKind.Whitespace?
    // Make sure all of the above are taken care of before you move on to a new highlighter.
    public enum SyntaxKind
    {
        Begin = 0,
        None = Begin,

        Annotation,
        BooleanLiteral,
        Comment,
        Eof,
        Identifier,
        Keyword,
        NullLiteral,
        NumericLiteral,
        Parenthesis,
        StringLiteral,

        End = StringLiteral
    }

    internal static class SyntaxKindExtensions
    {
        public static bool IsNone(this SyntaxKind kind) => kind == SyntaxKind.None;

        public static bool IsValid(this SyntaxKind kind) => kind >= SyntaxKind.Begin && kind <= SyntaxKind.End;
    }
}
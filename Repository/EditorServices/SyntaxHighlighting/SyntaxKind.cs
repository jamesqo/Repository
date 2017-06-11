namespace Repository.EditorServices.SyntaxHighlighting
{
    // TODO: Introduce SyntaxKind.Plaintext or Generic or None or Default
    // TODO: As for the above, probably nah. SyntaxKind.Class(Declaration), SyntaxKind.Member(Declaration/Invocation)?
    public enum SyntaxKind
    {
        Annotation,
        BooleanLiteral,
        Comment,
        Eof,
        Identifier,
        Keyword,
        NullLiteral,
        NumericLiteral,
        StringLiteral
    }
}
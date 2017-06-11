namespace Repository.Internal.SyntaxHighlighting
{
    // TODO: Introduce SyntaxKind.Plaintext or Generic or None or Default
    // TODO: As for the above, probably nah. SyntaxKind.Class(Declaration), SyntaxKind.Member(Declaration/Invocation)?
    internal enum SyntaxKind
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
namespace Repository.Internal.SyntaxHighlighting
{
    // TODO: Introduce SyntaxKind.Plaintext or Generic or None or Default
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
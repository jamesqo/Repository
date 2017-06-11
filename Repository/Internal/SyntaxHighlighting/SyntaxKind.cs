namespace Repository.Internal.SyntaxHighlighting
{
    // TODO: Introduce SyntaxKind.Plaintext or Generic or None or Default
    internal enum SyntaxKind
    {
        Annotation,
        Comment,
        Eof,
        Identifier,
        Keyword,
        NumericLiteral,
        StringLiteral
    }
}
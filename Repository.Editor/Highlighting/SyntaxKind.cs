﻿namespace Repository.Editor.Highlighting
{
    // TODO: As for the above, probably nah. SyntaxKind.Class(Declaration), SyntaxKind.Member(Declaration/Invocation)?
    // Make sure all of the above are taken care of before you move on to a new highlighter.
    public enum SyntaxKind
    {
        Begin = 0,
        None = Begin,

        Annotation,
        BooleanLiteral,
        Comment,
        ConstructorDeclaration,
        Eof,
        ExcludedCode,
        Identifier,
        Keyword,
        MethodDeclaration,
        MethodIdentifier,
        NullLiteral,
        NumericLiteral,
        Operator,
        ParameterDeclaration,
        Plaintext,
        StringLiteral,
        TypeDeclaration,
        TypeIdentifier,

        End = TypeIdentifier
    }

    internal static class SyntaxKindExtensions
    {
        public static bool IsNone(this SyntaxKind kind) => kind == SyntaxKind.None;

        public static bool IsValid(this SyntaxKind kind) => kind >= SyntaxKind.Begin && kind <= SyntaxKind.End;
    }
}
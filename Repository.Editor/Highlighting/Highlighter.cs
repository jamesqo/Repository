using System;
using Repository.Common.Validation;
using Repository.Editor.Internal.CSharp.Highlighting;
using Repository.Editor.Internal.Java.Highlighting;
using Repository.Editor.Internal.Plaintext.Highlighting;

namespace Repository.Editor.Highlighting
{
    public static class Highlighter
    {
        public static IHighlighter CSharp { get; } = new CSharpHighlighter();

        public static IHighlighter Java { get; } = new JavaHighlighter();

        public static IHighlighter Plaintext { get; } = new PlaintextHighlighter();

        public static IHighlighter FromLanguage(Language language)
        {
            Verify.Argument(language.IsValid(), nameof(language));

            switch (language)
            {
                case Language.CSharp:
                    return CSharp;
                case Language.Java:
                    return Java;
                case Language.Unknown:
                    return Plaintext;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
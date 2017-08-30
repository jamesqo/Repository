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

        public static IHighlighter FromFileExtension(string fileExtension)
        {
            Verify.NotNullOrEmpty(fileExtension, nameof(fileExtension));

            switch (fileExtension)
            {
                case "cs":
                    return CSharp;
                case "java":
                    return Java;
                case "txt":
                    return Plaintext;
            }

            return null;
        }

        public static IHighlighter FromFirstLine(string firstLine)
        {
            Verify.NotNull(firstLine, nameof(firstLine));

            return null;
        }
    }
}
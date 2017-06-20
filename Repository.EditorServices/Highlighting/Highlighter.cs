using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Repository.Common;
using Repository.EditorServices.Internal.CSharp.Highlighting;
using Repository.EditorServices.Internal.Java.Highlighting;
using Repository.EditorServices.Internal.Plaintext.Highlighting;

namespace Repository.EditorServices.Highlighting
{
    public static class Highlighter
    {
        public static IHighlighter CSharp { get; } = new CSharpHighlighter();

        public static IHighlighter Java { get; } = new JavaHighlighter();

        public static IHighlighter Plaintext { get; } = new PlaintextHighlighter();

        // TODO: Consider whether appropriate for this assembly.

        public static IHighlighter FromFileExtension(string fileExtension)
        {
            Verify.NotNullOrEmpty(fileExtension, nameof(fileExtension));

            switch (fileExtension)
            {
                // TODO: Not only .cs is used for C# files. Look at Linguist.
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

            // TODO: Detect shebangs, <?xml ...?>, maybe PHP, other stuff.
            return null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Repository.Common;
using Repository.EditorServices.Internal.Languages.Java.SyntaxHighlighting;
using Repository.EditorServices.Internal.Languages.Plaintext.SyntaxHighlighting;

namespace Repository.EditorServices.SyntaxHighlighting
{
    public static class SyntaxHighlighter
    {
        public static ISyntaxHighlighter Java { get; } = new JavaSyntaxHighlighter();

        public static ISyntaxHighlighter Plaintext { get; } = new PlaintextSyntaxHighlighter();

        // TODO: Consider whether appropriate for this assembly.

        public static ISyntaxHighlighter FromFileExtension(string fileExtension)
        {
            Verify.NotNullOrEmpty(fileExtension, nameof(fileExtension));

            switch (fileExtension)
            {
                case "java":
                    return Java;
                case "txt":
                    return Plaintext;
            }

            return null;
        }

        public static ISyntaxHighlighter FromFirstLine(string firstLine)
        {
            Verify.NotNull(firstLine, nameof(firstLine));

            // TODO: Detect shebangs, <?xml ...?>, maybe PHP, other stuff.
            return null;
        }
    }
}
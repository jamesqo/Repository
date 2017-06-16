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
    public static class SyntaxHighlighter
    {
        public static ISyntaxHighlighter CSharp { get; } = new CSharpSyntaxHighlighter();

        public static ISyntaxHighlighter Java { get; } = new JavaSyntaxHighlighter();

        public static ISyntaxHighlighter Plaintext { get; } = new PlaintextSyntaxHighlighter();

        // TODO: Consider whether appropriate for this assembly.

        public static ISyntaxHighlighter FromFileExtension(string fileExtension)
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

        public static ISyntaxHighlighter FromFirstLine(string firstLine)
        {
            Verify.NotNull(firstLine, nameof(firstLine));

            // TODO: Detect shebangs, <?xml ...?>, maybe PHP, other stuff.
            return null;
        }
    }
}
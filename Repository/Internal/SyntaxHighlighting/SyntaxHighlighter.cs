using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository.Internal.SyntaxHighlighting
{
    internal static class SyntaxHighlighter
    {
        public static ISyntaxHighlighter Plaintext { get; } = new PlaintextSyntaxHighlighter();

        public static ISyntaxHighlighter FromFileExtension(string fileExtension)
        {
            Verify.NotNullOrEmpty(fileExtension, nameof(fileExtension));

            switch (fileExtension)
            {
                case "java":
                    return new JavaSyntaxHighlighter();
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
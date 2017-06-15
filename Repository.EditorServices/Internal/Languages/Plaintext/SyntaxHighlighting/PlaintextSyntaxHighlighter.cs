using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Repository.EditorServices.SyntaxHighlighting;

namespace Repository.EditorServices.Internal.Languages.Plaintext.SyntaxHighlighting
{
    internal class PlaintextSyntaxHighlighter : ISyntaxHighlighter
    {
        public TResult Highlight<TResult>(string text, ISyntaxColorer<TResult> colorer)
        {
            colorer.Color(SyntaxKind.Identifier, 0, text.Length);
            return colorer.Result;
        }
    }
}
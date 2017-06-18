using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Repository.EditorServices.Highlighting;

namespace Repository.EditorServices.Internal.Plaintext.Highlighting
{
    internal class PlaintextSyntaxHighlighter : ISyntaxHighlighter
    {
        public TResult Highlight<TResult>(string text, ISyntaxColorer<TResult> colorer)
        {
            colorer.Color(SyntaxKind.Plaintext, text.Length);
            return colorer.Result;
        }
    }
}
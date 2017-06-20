﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Repository.EditorServices.Highlighting;

namespace Repository.EditorServices.Internal.Plaintext.Highlighting
{
    internal class PlaintextHighlighter : IHighlighter
    {
        public void Highlight(string text, ITextColorer colorer) => colorer.Color(SyntaxKind.Plaintext, text.Length);
    }
}
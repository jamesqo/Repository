using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository.Editor.Highlighting
{
    public interface IHighlighter
    {
        void Highlight(string text, ITextColorer colorer);
    }
}
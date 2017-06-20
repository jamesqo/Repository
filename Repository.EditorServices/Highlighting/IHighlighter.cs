using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository.EditorServices.Highlighting
{
    public interface IHighlighter
    {
        void Highlight(string text, ITextColorer colorer);
    }
}
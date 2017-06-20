using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository.EditorServices.Highlighting
{
    public interface IHighlighter
    {
        TResult Highlight<TResult>(string text, ITextColorer<TResult> colorer);
    }
}
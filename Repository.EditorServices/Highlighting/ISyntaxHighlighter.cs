using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository.EditorServices.Highlighting
{
    public interface ISyntaxHighlighter
    {
        TResult Highlight<TResult>(string text, ISyntaxColorer<TResult> colorer);
    }
}
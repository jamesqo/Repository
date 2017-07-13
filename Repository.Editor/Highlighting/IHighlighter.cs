using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Editor.Highlighting
{
    public interface IHighlighter
    {
        Task Highlight(string text, ITextColorer colorer);
    }
}
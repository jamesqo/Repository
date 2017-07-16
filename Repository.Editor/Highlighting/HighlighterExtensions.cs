using System.Threading;
using System.Threading.Tasks;

namespace Repository.Editor.Highlighting
{
    public static class HighlighterExtensions
    {
        public static Task Highlight(this IHighlighter highlighter, string text, ITextColorer colorer)
        {
            return highlighter.Highlight(text, colorer, CancellationToken.None);
        }
    }
}
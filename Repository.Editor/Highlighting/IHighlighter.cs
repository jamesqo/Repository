using System.Threading;
using System.Threading.Tasks;

namespace Repository.Editor.Highlighting
{
    public interface IHighlighter
    {
        Task Highlight(string text, ITextColorer colorer, CancellationToken cancellationToken);
    }
}
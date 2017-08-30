using System.Threading;
using System.Threading.Tasks;
using Repository.Editor.Highlighting;

namespace Repository.Editor.Internal.Plaintext.Highlighting
{
    internal class PlaintextHighlighter : IHighlighter
    {
        public Task Highlight(string text, ITextColorer colorer, CancellationToken cancellationToken)
        {
            return colorer.Color(SyntaxKind.Plaintext, text.Length, cancellationToken);
        }
    }
}
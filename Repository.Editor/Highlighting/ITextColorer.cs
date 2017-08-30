using System.Threading;
using System.Threading.Tasks;

namespace Repository.Editor.Highlighting
{
    public interface ITextColorer
    {
        Task Color(SyntaxKind kind, int count, CancellationToken cancellationToken);
    }
}

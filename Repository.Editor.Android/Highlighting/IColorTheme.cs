using Android.Graphics;
using Repository.Editor.Highlighting;

namespace Repository.Editor.Android.Highlighting
{
    public interface IColorTheme
    {
        Color BackgroundColor { get; }

        Color GetForegroundColor(SyntaxKind kind);
    }
}
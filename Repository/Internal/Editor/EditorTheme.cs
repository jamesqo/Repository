using Android.Graphics;
using Repository.Common;
using Repository.Internal.Editor.Highlighting;
using static System.Threading.LazyInitializer;

namespace Repository.Internal.Editor
{
    internal class EditorTheme
    {
        private static EditorTheme s_default;

        public static EditorTheme Default =>
            EnsureInitialized(ref s_default, () => new EditorTheme(ColorTheme.Default, Typefaces.Inconsolata));

        public EditorTheme(IColorTheme colors, Typeface typeface)
        {
            Verify.NotNull(colors, nameof(colors));
            Verify.NotNull(typeface, nameof(typeface));

            Colors = colors;
            Typeface = typeface;
        }

        public IColorTheme Colors { get; }

        public Typeface Typeface { get; }
    }
}
using Android.Graphics;
using Repository.Common.Validation;
using Repository.Editor.Android.Highlighting;

namespace Repository.Editor.Android
{
    public class EditorTheme
    {
        public static EditorTheme GetDefault(ITypefaceProvider typefaces)
            => new EditorTheme(ColorTheme.Default, typefaces.Inconsolata);

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
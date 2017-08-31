using Repository.Editor.Android.Internal.Editor.Highlighting;

namespace Repository.Editor.Android.Highlighting
{
    public static class ColorTheme
    {
        public static IColorTheme Default => Monokai;

        public static IColorTheme Monokai { get; } = new MonokaiColorTheme();
    }
}
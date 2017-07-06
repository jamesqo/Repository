using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;
using Repository.Common;
using Repository.Internal.Editor.Highlighting;

namespace Repository.Internal.Editor
{
    internal class EditorTheme
    {
        public static EditorTheme Default => new EditorTheme(ColorTheme.Default, Typefaces.Inconsolata);

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
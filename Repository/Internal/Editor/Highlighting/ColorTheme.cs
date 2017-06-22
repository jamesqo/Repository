using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository.Internal.Editor.Highlighting
{
    internal static class ColorTheme
    {
        public static IColorTheme Default => Monokai;

        public static IColorTheme Monokai { get; } = new MonokaiColorTheme();
    }
}
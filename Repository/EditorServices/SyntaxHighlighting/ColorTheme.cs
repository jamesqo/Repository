using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Repository.Internal.EditorServices.SyntaxHighlighting;

namespace Repository.EditorServices.SyntaxHighlighting
{
    public static class ColorTheme
    {
        public static IColorTheme Default => Monokai;

        public static IColorTheme Monokai { get; } = new MonokaiColorTheme();
    }
}
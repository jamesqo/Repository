using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Repository.Internal.EditorServices.Highlighting;

namespace Repository.EditorServices.Highlighting
{
    public static class ColorTheme
    {
        public static IColorTheme Default => Monokai;

        public static IColorTheme Monokai { get; } = new MonokaiColorTheme();
    }
}
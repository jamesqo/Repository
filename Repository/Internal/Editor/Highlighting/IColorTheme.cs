using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;
using Repository.Editor.Highlighting;

namespace Repository.Internal.Editor.Highlighting
{
    internal interface IColorTheme
    {
        Color BackgroundColor { get; }

        Color GetForegroundColor(SyntaxKind kind);
    }
}
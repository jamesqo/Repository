using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;

namespace Repository.EditorServices.Highlighting
{
    public interface IColorTheme
    {
        Color BackgroundColor { get; }

        Color GetForegroundColor(SyntaxKind kind);
    }
}
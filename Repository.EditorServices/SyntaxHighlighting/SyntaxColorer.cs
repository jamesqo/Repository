using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;
using Android.Text.Style;
using Repository.Internal.EditorServices.SyntaxHighlighting;

namespace Repository.EditorServices.SyntaxHighlighting
{
    public abstract class SyntaxColorer : ISyntaxStyler
    {
        public static SyntaxColorer Default => Monokai;

        public static SyntaxColorer Monokai { get; } = new MonokaiSyntaxColorer();

        public abstract Color BackgroundColor { get; }

        public abstract Color GetForegroundColor(SyntaxKind kind);

        Java.Lang.Object ISyntaxStyler.GetSpan(SyntaxKind kind) => new ForegroundColorSpan(GetForegroundColor(kind));
    }
}
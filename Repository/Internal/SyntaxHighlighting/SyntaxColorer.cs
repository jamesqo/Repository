using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;
using Android.Text.Style;

namespace Repository.Internal.SyntaxHighlighting
{
    internal abstract class SyntaxColorer : ISyntaxStyler
    {
        public static SyntaxColorer Default => Monokai;

        public static SyntaxColorer Monokai { get; } = new MonokaiSyntaxColorer();

        public abstract Color GetColor(SyntaxKind kind);

        Java.Lang.Object ISyntaxStyler.GetSpan(SyntaxKind kind) => new ForegroundColorSpan(GetColor(kind));
    }

    internal class MonokaiSyntaxColorer : SyntaxColorer
    {
        public override Color GetColor(SyntaxKind kind)
        {
            // TODO: Based off image from https://darekkay.com/2014/11/23/monokai-theme-intellij/
            switch (kind)
            {
                case SyntaxKind.Annotation: return Color.Yellow;
                case SyntaxKind.BooleanLiteral: return Color.HotPink;
                case SyntaxKind.Comment: return Color.LightGray;
                case SyntaxKind.Eof: return default(Color);
                case SyntaxKind.Identifier: return Color.White;
                case SyntaxKind.Keyword: return Color.HotPink;
                case SyntaxKind.NullLiteral: return Color.HotPink;
                case SyntaxKind.NumericLiteral: return Color.Purple;
                case SyntaxKind.StringLiteral: return Color.Yellow;
                default: throw new NotSupportedException();
            }
        }
    }
}
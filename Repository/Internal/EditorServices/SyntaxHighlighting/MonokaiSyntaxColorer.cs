using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;
using Repository.EditorServices.SyntaxHighlighting;

namespace Repository.Internal.EditorServices.SyntaxHighlighting
{
    internal class MonokaiSyntaxColorer : SyntaxColorer
    {
        public override Color BackgroundColor => Color.Black;

        public override Color GetForegroundColor(SyntaxKind kind)
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
                case SyntaxKind.Parenthesis: return Color.White;
                case SyntaxKind.StringLiteral: return Color.Yellow;
                case SyntaxKind.TypeDeclaration: return Color.LightGreen;
                case SyntaxKind.TypeIdentifier: return Color.SkyBlue;
                default: throw new NotSupportedException();
            }
        }
    }
}
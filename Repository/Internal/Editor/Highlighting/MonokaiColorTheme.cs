using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;
using Repository.Editor.Highlighting;

namespace Repository.Internal.Editor.Highlighting
{
    internal class MonokaiColorTheme : IColorTheme
    {
        public Color BackgroundColor => Color.Black;

        public Color GetForegroundColor(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.Annotation: return Color.SkyBlue;
                case SyntaxKind.BooleanLiteral: return Color.MediumPurple;
                case SyntaxKind.Comment: return Color.Gray;
                case SyntaxKind.ConstructorDeclaration: return Color.LightGreen;
                case SyntaxKind.Eof: return default(Color);
                case SyntaxKind.Identifier: return Color.White;
                case SyntaxKind.Keyword: return Color.HotPink;
                case SyntaxKind.MethodDeclaration: return Color.LightGreen;
                case SyntaxKind.MethodIdentifier: return Color.LightGreen;
                case SyntaxKind.NullLiteral: return Color.MediumPurple;
                case SyntaxKind.NumericLiteral: return Color.MediumPurple;
                case SyntaxKind.Operator: return Color.HotPink;
                case SyntaxKind.ParameterDeclaration: return Color.Orange;
                case SyntaxKind.Plaintext: return Color.White;
                case SyntaxKind.StringLiteral: return Color.SandyBrown;
                case SyntaxKind.TypeDeclaration: return Color.LightGreen;
                case SyntaxKind.TypeIdentifier: return Color.SkyBlue;
                default: throw new NotSupportedException();
            }
        }
    }
}
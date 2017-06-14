﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;
using Repository.EditorServices.SyntaxHighlighting;

namespace Repository.EditorServices.Internal.SyntaxHighlighting
{
    internal class MonokaiSyntaxColorer : SyntaxColorer
    {
        public override Color BackgroundColor => Color.Black;

        public override Color GetForegroundColor(SyntaxKind kind)
        {
            // TODO: Based off VSCode's Monokai theme
            switch (kind)
            {
                case SyntaxKind.Annotation: return Color.SkyBlue;
                case SyntaxKind.BooleanLiteral: return Color.Purple;
                case SyntaxKind.Comment: return Color.Gray;
                case SyntaxKind.ConstructorDeclaration: return Color.LightGreen;
                case SyntaxKind.Eof: return default(Color);
                case SyntaxKind.Identifier: return Color.White;
                case SyntaxKind.Keyword: return Color.HotPink;
                case SyntaxKind.MethodDeclaration: return Color.LightGreen;
                case SyntaxKind.MethodIdentifier: return Color.LightGreen;
                case SyntaxKind.NullLiteral: return Color.Purple;
                case SyntaxKind.NumericLiteral: return Color.Purple;
                case SyntaxKind.ParameterDeclaration: return Color.Orange;
                case SyntaxKind.Parenthesis: return Color.White;
                case SyntaxKind.StringLiteral: return Color.Beige;
                case SyntaxKind.TypeDeclaration: return Color.LightGreen;
                case SyntaxKind.TypeIdentifier: return Color.LightGreen;
                default: throw new NotSupportedException();
            }
        }
    }
}
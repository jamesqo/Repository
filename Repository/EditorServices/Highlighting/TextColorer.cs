using System;
using System.Collections.Generic;
using System.Diagnostics;
using Android.Graphics;
using Repository.Common;
using Repository.Internal;
using Coloring = System.Int64;

namespace Repository.EditorServices.Highlighting
{
    public class TextColorer : ITextColorer<ColoredText>
    {
        private readonly string _text;
        private readonly IColorTheme _theme;
        private readonly ArrayBuilder<Coloring> _colorings;

        private TextColorer(string text, IColorTheme theme)
        {
            Verify.NotNull(text, nameof(text));
            Verify.NotNull(theme, nameof(theme));

            _text = text;
            _theme = theme;
            _colorings = new ArrayBuilder<Coloring>();
        }

        public static TextColorer Create(string text, IColorTheme theme) => new TextColorer(text, theme);

        public ColoredText Result => new ColoredText(_text, _colorings.Array, _colorings.Count);

        public void Color(SyntaxKind kind, int count)
        {
            var color = _theme.GetForegroundColor(kind);
            _colorings.Add(MakeColoring(color, count));
        }

        private static Coloring MakeColoring(Color color, int count)
        {
            Debug.Assert(count >= 0);
            return ((long)color.ToArgb() << 32) | (uint)count;
        }
    }
}
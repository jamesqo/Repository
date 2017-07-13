using System;
using System.Collections.Generic;
using System.Diagnostics;
using Repository.Common;
using Repository.Editor.Highlighting;
using Repository.Internal.Java;
using Repository.JavaInterop;

namespace Repository.Internal.Editor.Highlighting
{
    internal class TextColorer : ITextColorer
    {
        private readonly ColoredText _source;
        private readonly IColorTheme _theme;

        private TextColorer(string text, IColorTheme theme)
        {
            Verify.NotNull(text, nameof(text));
            Verify.NotNull(theme, nameof(theme));

            _source = ColoredTextSource.Create(text);
            _theme = theme;
        }

        public static TextColorer Create(string text, IColorTheme theme) => new TextColorer(text, theme);

        public ColoredText InitialWindow => _source.InitialWindow;

        public void Color(SyntaxKind kind, int count)
        {
            Debug.Assert(count > 0);

            var color = _theme.GetForegroundColor(kind);
            _source.Color(color, count);
        }
    }
}
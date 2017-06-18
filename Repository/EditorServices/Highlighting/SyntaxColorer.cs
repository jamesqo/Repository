﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Text;
using Android.Text.Style;
using Repository.Common;
using Repository.Internal.EditorServices.Highlighting;

namespace Repository.EditorServices.Highlighting
{
    public class SyntaxColorer : ISyntaxColorer<SpannableText>
    {
        private readonly SpannableText _text;
        private readonly IColorTheme _theme;

        private int _index;

        private SyntaxColorer(string text, IColorTheme theme)
        {
            Verify.NotNull(text, nameof(text));
            Verify.NotNull(theme, nameof(theme));

            _text = SpannableText.Create(text);
            _theme = theme;
        }

        public static SyntaxColorer Create(string text, IColorTheme theme) => new SyntaxColorer(text, theme);

        public SpannableText Result => _text;

        public void Color(SyntaxKind kind, int count)
        {
            var color = _theme.GetForegroundColor(kind);
            var span = new ForegroundColorSpan(color);
            _text.SetSpan(span, _index, _index + count, SpanTypes.InclusiveExclusive);
            _index += count;
        }
    }
}
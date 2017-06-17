using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using Repository.Common;
using Repository.Internal;

namespace Repository.EditorServices.Highlighting
{
    public class SyntaxColorer : ISyntaxColorer<SpannableString>
    {
        private readonly SpannableString _text;
        private readonly IColorTheme _theme;
        private readonly Dictionary<Color, ForegroundColorSpan> _spanCache;

        private SyntaxColorer(string text, IColorTheme theme)
        {
            Verify.NotNull(text, nameof(text));
            Verify.NotNull(theme, nameof(theme));

            _text = new SpannableString(text);
            _theme = theme;
            _spanCache = new Dictionary<Color, ForegroundColorSpan>();
        }

        public static SyntaxColorer Create(string text, IColorTheme theme) => new SyntaxColorer(text, theme);

        public SpannableString Result => _text;

        public void Color(SyntaxKind kind, int index, int count)
        {
            var color = _theme.GetForegroundColor(kind);
            var span = _spanCache.GetOrAdd(color, _ => new ForegroundColorSpan(color));
            _text.SetSpan(span, index, index + count, SpanTypes.InclusiveExclusive);
        }
    }
}
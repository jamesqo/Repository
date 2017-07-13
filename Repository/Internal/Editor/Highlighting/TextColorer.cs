using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Android.Graphics;
using Repository.Common;
using Repository.Editor.Highlighting;
using Repository.Internal.Java;
using Repository.JavaInterop;

namespace Repository.Internal.Editor.Highlighting
{
    internal class TextColorer : ITextColorer
    {
        private const int BatchCount = 256;

        private readonly ColoredText _text;
        private readonly IColorTheme _theme;
        private readonly int _textLength;

        private ByteBufferWrapper _colorings;
        private int _index;

        private TextColorer(string text, IColorTheme theme)
        {
            Verify.NotNull(text, nameof(text));
            Verify.NotNull(theme, nameof(theme));

            _text = new ColoredText(text);
            _theme = theme;
            // Don't hold a reference to the text, let the GC collect it ASAP.
            _textLength = text.Length;
            _colorings = new ByteBufferWrapper(BatchCount * 8);
        }

        public static TextColorer Create(string text, IColorTheme theme) => new TextColorer(text, theme);

        public ColoredText Text => _text;

        public Task Color(SyntaxKind kind, int count)
        {
            Debug.Assert(count > 0);

            var color = _theme.GetForegroundColor(kind);
            _colorings.Add(Coloring.Create(color, count).ToLong());
            _index += count;

            return _colorings.IsFull
                ? Flush()
                : _index == _textLength
                ? Dispose()
                : Task.CompletedTask;
        }

        private async Task Dispose()
        {
            Debug.Assert(_colorings != null);

            await Flush();
            _colorings.Dispose();
            _colorings = null;
        }

        private async Task Flush()
        {
            int byteCount = _colorings.ByteCount;
            Debug.Assert(byteCount > 0 && byteCount % 8 == 0);

            var colorings = ColoringList.FromBufferSpan(
                _colorings.Unwrap(), 0, byteCount / 8);
            _text.ColorWith(colorings); // Segments are separated by '\n'.
            _colorings.Clear();

            await Task.Yield();
        }
    }
}
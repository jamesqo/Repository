using System;
using System.Collections.Generic;
using System.Diagnostics;
using Android.Graphics;
using Repository.Common;
using Repository.Internal;
using Repository.Internal.EditorServices.Highlighting;
using Repository.JavaInterop;

namespace Repository.EditorServices.Highlighting
{
    public class TextColorer : ITextColorer, IDisposable
    {
        private const int BatchCount = 256;

        private readonly IColorTheme _theme;
        private readonly WrappedByteBuffer _colorings;
        private readonly ColoredText _text;

        private TextColorer(string text, IColorTheme theme)
        {
            Verify.NotNull(text, nameof(text));
            Verify.NotNull(theme, nameof(theme));

            _theme = theme;
            _colorings = new WrappedByteBuffer(BatchCount * 8);
            _text = new ColoredText(text);
        }

        public static TextColorer Create(string text, IColorTheme theme) => new TextColorer(text, theme);

        public ColoredText Text => _text;

        public void Color(SyntaxKind kind, int count)
        {
            Debug.Assert(count > 0);

            var color = _theme.GetForegroundColor(kind);
            _colorings.Add(MakeColoring(color, count));

            if (_colorings.IsFull)
            {
                Flush();
            }
        }

        public void Dispose()
        {
            // TODO: Make sure we're not disposed twice?
            Flush();
            _colorings.Dispose();
        }

        private void Flush()
        {
            int byteCount = _colorings.ByteCount;
            if (byteCount > 0)
            {
                // TODO: Rename ColoredText => +Stream?
                _text.Receive(_colorings.Unwrap(), byteCount / 8);
                _colorings.Clear();
            }
        }

        private static long MakeColoring(Color color, int count)
        {
            return ((long)color.ToArgb() << 32) | (uint)count;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Android.Graphics;
using Repository.Common;
using Repository.Editor.Highlighting;
using Repository.Internal.Java;
using Repository.JavaInterop;

namespace Repository.Internal.Editor.Highlighting
{
    internal class TextColorer : ITextColorer, IDisposable
    {
        // TODO: Consider increasing the batch size geometrically, which would cause O(log n)
        // callbacks to be posted to the UI thread instead of O(n).
        // Or not. One reason for keeping it small is to reduce lock contention.
        private const int BatchCount = 256; // TODO: Remove?
        private const int MaxLinesPerSegment = 50;

        private readonly ColoredTextList _segments;
        private readonly int _lineCount;
        private readonly IColorTheme _theme;
        private readonly ByteBufferWrapper _colorings;

        private TextColorer(string text, IColorTheme theme)
        {
            Verify.NotNull(text, nameof(text));
            Verify.NotNull(theme, nameof(theme));

            _segments = ColoredTextList.Create(MakeSegments(text));
            _lineCount = text.LineCount();
            _theme = theme;
            _colorings = new ByteBufferWrapper(BatchCount * 8);
        }

        public static TextColorer Create(string text, IColorTheme theme) => new TextColorer(text, theme);

        // TODO: Cleanup. Maybe cache SegmentCount instead of _lineCount.
        public int SegmentCount => (int)Math.Ceiling((double)_lineCount / MaxLinesPerSegment);

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

        public ColoredText GetSegment(int index) => _segments.GetText(index);

        private void Flush()
        {
            int byteCount = _colorings.ByteCount;
            if (byteCount > 0)
            {
                var colorings = ColoringList.FromBufferSpan(
                    _colorings.Unwrap(), 0, byteCount / 8);
                _segments.ColorWith(colorings);
                _colorings.Clear();
            }
        }

        private static long MakeColoring(Color color, int count)
        {
            return ((long)color.ToArgb() << 32) | (uint)count;
        }

        private static IEnumerable<string> MakeSegments(string content)
        {
            int end = -1;
            while (true)
            {
                int start = end + 1;
                int newLine = content.IndexOfNth('\n', MaxLinesPerSegment, start);
                if (newLine == -1)
                {
                    // TODO: Should a segment be created for the empty string?
                    // Currently, the answer is yes.
                    // Will wrap_content allow the user to see/tap on the EditText?
                    end = content.Length;
                    yield return content.Substring(start, end - start);
                    break;
                }

                end = newLine;
                // TODO: Investigate if strings are copied when they cross over to Java.
                // TODO: Return a StringSegment instead to avoid copying?
                yield return content.Substring(start, end - start);
            }
        }
    }
}
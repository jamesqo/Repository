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
    internal class TextColorer : ITextColorer
    {
        private const int BatchCount = 256;
        private const int MaxLinesPerSegment = 50;

        private readonly ColoredTextList _segments;
        private readonly int _segmentCount;
        private readonly IColorTheme _theme;

        private ByteBufferWrapper _colorings;

        private TextColorer(string text, IColorTheme theme)
        {
            Verify.NotNull(text, nameof(text));
            Verify.NotNull(theme, nameof(theme));

            _segments = ColoredTextList.Create(MakeSegments(text));
            _segmentCount = MathUtilities.Ceiling(text.LineCount(), MaxLinesPerSegment);
            _theme = theme;
        }

        public static TextColorer Create(string text, IColorTheme theme) => new TextColorer(text, theme);

        public int SegmentCount => _segmentCount;

        public void Color(SyntaxKind kind, int count)
        {
            Debug.Assert(count > 0);

            var color = _theme.GetForegroundColor(kind);
            _colorings.Add(Coloring.Create(color, count).ToLong());

            if (_colorings.IsFull)
            {
                Flush();
            }
        }

        public ColoredText GetSegment(int index) => _segments.GetText(index);

        public IDisposable Setup()
        {
            Debug.Assert(_colorings == null);

            _colorings = new ByteBufferWrapper(BatchCount * 8);
            return Disposable.Create(Teardown);
        }

        private void Flush()
        {
            int byteCount = _colorings.ByteCount;
            if (byteCount > 0)
            {
                var colorings = ColoringList.FromBufferSpan(
                    _colorings.Unwrap(), 0, byteCount / 8);
                _segments.ColorWith(colorings, separatorLength: 1); // Segments are separated by '\n'.
                _colorings.Clear();
            }
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
                // TODO: Return a StringSegment instead to avoid copying?
                yield return content.Substring(start, end - start);
            }
        }

        private void Teardown()
        {
            Debug.Assert(_colorings != null);

            Flush();
            _colorings.Dispose();
            _colorings = null;
        }
    }
}
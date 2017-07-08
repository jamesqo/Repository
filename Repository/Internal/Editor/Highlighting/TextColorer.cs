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

        private readonly string _rawText;
        private readonly ColoredTextList _segments;
        private readonly int _segmentCount;
        private readonly IColorTheme _theme;

        private ByteBufferWrapper _colorings;
        private int _index;

        private int _indexToPass;
        private Action _indexPassedCallback;

        private TextColorer(string text, IColorTheme theme)
        {
            Verify.NotNull(text, nameof(text));
            Verify.NotNull(theme, nameof(theme));

            _rawText = text;
            _segments = ColoredTextList.Create(MakeSegments(text));
            _segmentCount = MathUtilities.Ceiling(text.LineCount(), MaxLinesPerSegment);
            _theme = theme;
            _indexToPass = -1;
        }

        public static TextColorer Create(string text, IColorTheme theme) => new TextColorer(text, theme);

        public int SegmentCount => _segmentCount;

        public void Color(SyntaxKind kind, int count)
        {
            Debug.Assert(count > 0);

            var color = _theme.GetForegroundColor(kind);
            _colorings.Add(Coloring.Create(color, count).ToLong());
            _index += count;

            if (_colorings.IsFull)
            {
                Flush();
            }
        }

        public ColoredText GetSegment(int index) => _segments[index];

        /// <summary>
        /// Gets the exclusive end of the segment at <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the segment.</param>
        /// <returns>
        /// The end of the segment, or the length of the text if the segment was not found.
        /// </returns>
        public int GetSegmentEnd(int index)
        {
            int end = _rawText.IndexOfNth('\n', (index + 1) * MaxLinesPerSegment);
            return end == -1 ? _rawText.Length : end;
        }

        public IDisposable Setup()
        {
            Debug.Assert(_colorings == null);

            _colorings = new ByteBufferWrapper(BatchCount * 8);
            return Disposable.Create(Teardown);
        }

        public void WhenIndexPassed(int index, Action callback)
        {
            // Currently, we only need to handle a single index/callback at a time.
            Debug.Assert(_indexToPass == -1);
            Debug.Assert(_indexPassedCallback == null);

            _indexToPass = index;
            _indexPassedCallback = callback;
        }

        private void Flush()
        {
            int byteCount = _colorings.ByteCount;
            Debug.Assert(byteCount > 0);

            var colorings = ColoringList.FromBufferSpan(
                _colorings.Unwrap(), 0, byteCount / 8);
            _segments.ColorWith(colorings, separatorLength: 1); // Segments are separated by '\n'.
            _colorings.Clear();

            if (_indexToPass != -1 && _index >= _indexToPass)
            {
                RaiseIndexPassed();
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

        private void RaiseIndexPassed()
        {
            Debug.Assert(_indexToPass != -1);
            Debug.Assert(_indexPassedCallback != null);

            _indexToPass = -1;
            _indexPassedCallback();
            _indexPassedCallback = null;
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
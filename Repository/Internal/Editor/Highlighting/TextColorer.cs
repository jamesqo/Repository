using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Repository.Common;
using Repository.Editor.Highlighting;
using Repository.Internal.Java;
using Repository.Internal.Threading;
using Repository.JavaInterop;

namespace Repository.Internal.Editor.Highlighting
{
    internal class TextColorer : ITextColorer
    {
        private const int BatchCount = 256;

        private readonly ColoredText _text;
        private readonly IColorTheme _theme;

        private ByteBufferWrapper _colorings;

        private TextColorer(string text, IColorTheme theme)
        {
            Verify.NotNull(text, nameof(text));
            Verify.NotNull(theme, nameof(theme));

            _text = new ColoredText(text);
            _theme = theme;
        }

        public static TextColorer Create(string text, IColorTheme theme) => new TextColorer(text, theme);

        public ColoredText Text => _text;

        public Task Color(SyntaxKind kind, int count)
        {
            Debug.Assert(count > 0);

            var color = _theme.GetForegroundColor(kind);
            _colorings.Add(Coloring.Create(color, count).ToLong());

            return _colorings.IsFull
                ? FlushAsync()
                : Task.CompletedTask;
        }

        public IDisposable Setup()
        {
            Debug.Assert(_colorings == null);

            _colorings = new ByteBufferWrapper(BatchCount * 8);
            return Disposable.Create(Teardown);
        }

        private void Flush()
        {
            int byteCount = _colorings.ByteCount;
            Debug.Assert(byteCount > 0 && byteCount % 8 == 0);

            var colorings = ColoringList.FromBufferSpan(
                _colorings.Unwrap(), 0, byteCount / 8);
            _text.ColorWith(colorings); // Segments are separated by '\n'.
            _colorings.Clear();
        }

        private async Task FlushAsync()
        {
            Flush();
            // TODO: Ensure await Task.Yield() gives worse results.
            await UIThreadUtilities.Yield();
        }

        private void Teardown()
        {
            Debug.Assert(_colorings != null);

            // Control will soon be returned to the caller, so there's no point in yielding.
            // Call the synchronous version of Flush().
            Flush();
            _colorings.Dispose();
            _colorings = null;
        }
    }
}
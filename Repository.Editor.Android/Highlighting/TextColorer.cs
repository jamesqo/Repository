﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Repository.Common.Validation;
using Repository.Editor.Android.Internal;
using Repository.Editor.Android.Internal.Editor.Highlighting;
using Repository.Editor.Android.Internal.JavaInterop;
using Repository.Editor.Android.Internal.Threading;
using Repository.Editor.Android.Threading;
using Repository.Editor.Highlighting;
using Repository.JavaInterop;

namespace Repository.Editor.Android.Highlighting
{
    public class TextColorer : ITextColorer
    {
        private readonly EditorText _text;
        private readonly IColorTheme _theme;
        private readonly IYielder _yielder;

        private NativeByteBuffer _colorings;

        public TextColorer(string text, IColorTheme theme, IYielder yielder = null)
        {
            Verify.NotNull(text, nameof(text));
            Verify.NotNull(theme, nameof(theme));

            _text = new EditorText(text);
            _theme = theme;
            _yielder = yielder ?? DefaultYielder.Instance;
        }

        public EditorText Text => _text;

        public IColorTheme Theme => _theme;

        public Task Color(SyntaxKind kind, int count, CancellationToken cancellationToken)
        {
            Verify.Argument(kind.IsValid(), nameof(kind));
            Verify.InRange(count > 0, nameof(count));

            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(cancellationToken);
            }

            var color = _theme.GetForegroundColor(kind);
            new Coloring(color, count).WriteTo(_colorings);

            return _colorings.IsFull
                ? FlushAsync()
                : Task.CompletedTask;
        }

        /// <summary>
        /// Sets up this colorer for highlighting.
        /// Must be called before <see cref="Color"/> is called.
        /// </summary>
        /// <param name="flushFrequency">
        /// The number of times <see cref="Color"/> is called in between <see cref="FlushAsync"/> calls.
        /// Set this number lower to yield to pending work on the UI thread more often.
        /// </param>
        /// <returns>
        /// A disposable that undoes the work of this method.
        /// </returns>
        public IDisposable Setup(int flushFrequency)
        {
            Verify.ValidState(_colorings == null, "Only one highlight may be in progress at a time.");

            _colorings = new NativeByteBuffer(flushFrequency * Coloring.Size);
            return new Disposable(Teardown);
        }

        private void Flush()
        {
            int byteCount = _colorings.ByteCount;
            Debug.Assert(byteCount % Coloring.Size == 0);

            if (byteCount > 0)
            {
                var colorings = ColoringList.FromBufferSpan(
                    _colorings.Unwrap(), 0, byteCount / Coloring.Size);
                _text.AddColorings(colorings);
                _colorings.Clear();
            }
        }

        private async Task FlushAsync()
        {
            Flush();

            // This line is extremely important!
            // It interrupts our highlighting work at fixed intervals, giving the UI thread a chance
            // to run pending work such as input/rendering code, which keeps the app responsive.
            // Without it, user input would be ignored, and the app would freeze until all text was highlighted.
            await _yielder.Yield();
        }

        private void Teardown()
        {
            Verify.ValidState(_colorings != null, "Dispose was called twice.");

            // Control will soon be returned to the caller, so there's no point in yielding.
            // Call the synchronous version of Flush().
            Flush();
            _text.ResetColorCursor();

            _colorings.Dispose();
            _colorings = null;
        }
    }
}
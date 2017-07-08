using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Repository.Common;
using Repository.Internal.Java;
using Repository.JavaInterop;

namespace Repository.Internal.Editor.Highlighting
{
    internal class ColoredTextList
    {
        private readonly LazyList<ColoredText> _texts;
        private int _currentTextIndex; // Index of text that's currently being colored.

        private ColoredTextList(IEnumerable<string> texts)
        {
            Verify.NotNull(texts, nameof(texts));

            _texts = texts.Select(t => new ColoredText(t)).ToLazyList();
        }

        public static ColoredTextList Create(IEnumerable<string> texts) => new ColoredTextList(texts);

        public void ColorWith(ColoringList colorings, int separatorLength)
        {
            // Currently, the separator between segments is \n.
            // Handling separators with 2+ chars would require more involved logic.
            Debug.Assert(separatorLength == 1);

            void SkipSeparator()
            {
                // Remove the separator from consideration for the next coloring.
                // The separator is implicitly there, but isn't part of the text of any segment.
                var next = colorings[0];
                if (next.Count == 1)
                {
                    // The separator spans the entire coloring. Remove the coloring.
                    colorings = colorings.Slice(1);
                }
                else
                {
                    // Trim the coloring so it doesn't include the separator.
                    colorings[0] = next.WithCount(next.Count - 1);
                }
            }

            while (true)
            {
                var text = GetText(_currentTextIndex);
                int processed = text.ColorWith(colorings);
                if (processed == colorings.Count)
                {
                    // We've exhausted all of the colorings.
                    break;
                }

                // We've finished coloring this text. Move to the next one.
                colorings = colorings.Slice(processed);
                SkipSeparator();
                _currentTextIndex++;
            }
        }

        public ColoredText GetText(int index) => _texts.ElementAt(index);
    }
}
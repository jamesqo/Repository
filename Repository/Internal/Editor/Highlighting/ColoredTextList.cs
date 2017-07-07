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

        public void ColorWith(ColoringList colorings)
        {
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
                _currentTextIndex++;
            }
        }

        public ColoredText GetText(int index) => _texts.ElementAt(index);
    }
}
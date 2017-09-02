using Repository.Common.Validation;
using Repository.JavaInterop;

namespace Repository.Editor.Android.UnitTests.TestInternal.JavaInterop
{
    internal class TestCursor
    {
        private readonly EditorText _text;

        private int _index;

        internal TestCursor(EditorText text, int index)
        {
            Verify.NotNull(text, nameof(text));
            Verify.InRange(index >= 0 && index <= text.Length(), nameof(index));

            _text = text;
            _index = index;
        }

        public TestCursor Delete(string text)
        {
            var (start, end) = (_index, _index + text.Length);
            Verify.Argument(text == _text.SubSequence(start, end), nameof(text));

            _text.Delete(start, end);
            return this;
        }

        public TestCursor SkipWhitespace()
        {
            int length = _text.Length();
            while (_index < length && char.IsWhiteSpace(_text.CharAt(_index)))
            {
                _index++;
            }

            return this;
        }
    }
}

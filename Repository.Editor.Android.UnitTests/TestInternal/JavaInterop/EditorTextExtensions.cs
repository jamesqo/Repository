using Repository.Common.Validation;
using Repository.JavaInterop;

namespace Repository.Editor.Android.UnitTests.TestInternal.JavaInterop
{
    internal static class EditorTextExtensions
    {
        public static TestCursor GetCursor(this EditorText text, int index)
        {
            return new TestCursor(text, index);
        }

        public static TestCursor GetStartCursor(this EditorText text) => text.GetCursor(0);

        public static TestCursor GetStartCursor(this EditorText text, string subtext)
        {
            Verify.NotNullOrEmpty(text, nameof(text));
            Verify.NotNullOrEmpty(subtext, nameof(subtext));

            int index = text.ToString().IndexOf(subtext);
            Verify.ValidState(index != -1, $"{nameof(subtext)}: {subtext} is not a substring of {nameof(text)}: {text}");
            return text.GetCursor(index);
        }
    }
}

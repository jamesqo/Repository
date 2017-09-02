using Repository.JavaInterop;

namespace Repository.Editor.Android.UnitTests.TestInternal.JavaInterop
{
    internal static class EditorTextExtensions
    {
        public static TestCursor GetCursor(this EditorText text, int index)
        {
            return new TestCursor(text, index);
        }
    }
}
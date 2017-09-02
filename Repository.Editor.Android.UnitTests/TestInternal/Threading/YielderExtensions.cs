using Repository.Editor.Android.Threading;

namespace Repository.Editor.Android.UnitTests.TestInternal.Threading
{
    internal static class YielderExtensions
    {
        public static CancelAfterNYielder CancelAfter(this IYielder yielder, int n)
        {
            return new CancelAfterNYielder(yielder, n);
        }
    }
}

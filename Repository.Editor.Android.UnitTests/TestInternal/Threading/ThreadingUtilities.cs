using System.Threading;
using System.Threading.Tasks;

namespace Repository.Editor.Android.UnitTests.TestInternal.Threading
{
    internal static class ThreadingUtilities
    {
        public static Task CanceledTask { get; } = Task.FromCanceled(new CancellationToken(true));
    }
}
using Android.OS;

namespace Repository.Internal.Threading
{
    internal static class ThreadingUtilities
    {
        public static Handler UIThreadHandler { get; } = new Handler(Looper.MainLooper);
    }
}
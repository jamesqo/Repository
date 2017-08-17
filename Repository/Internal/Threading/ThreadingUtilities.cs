using System;
using Android.OS;

namespace Repository.Internal.Threading
{
    internal static class ThreadingUtilities
    {
        private static Handler UIThreadHandler { get; } = new Handler(Looper.MainLooper);

        public static bool PostToUIThread(Action action) => UIThreadHandler.Post(action);
    }
}
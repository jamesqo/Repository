using System;
using Android.OS;

namespace Repository.Internal.Threading
{
    internal static class ThreadingUtilities
    {
        /// <summary>
        /// A <see cref="Handler"/> associated with the UI thread.
        /// </summary>
        private static Handler UIThreadHandler { get; } = new Handler(Looper.MainLooper);

        /// <summary>
        /// Posts an action to the UI thread.
        /// </summary>
        /// <param name="action">The action.</param>
        public static bool PostToUIThread(Action action) => UIThreadHandler.Post(action);
    }
}
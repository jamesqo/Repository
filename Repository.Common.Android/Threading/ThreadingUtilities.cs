using System;
using Android.OS;

namespace Repository.Common.Android.Threading
{
    public static class ThreadingUtilities
    {
        public static bool IsRunningOnUIThread => Looper.MainLooper == Looper.MyLooper();

        /// <summary>
        /// Posts an action to the current thread.
        /// </summary>
        /// <param name="action">The action.</param>
        public static bool Post(Action action)
        {
            var handler = new Handler(Looper.MyLooper());
            return handler.Post(action);
        }
    }
}
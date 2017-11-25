using System;
using Android.OS;

namespace Repository.Editor.Android.Internal.Threading
{
    internal static class ThreadingUtilities
    {
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
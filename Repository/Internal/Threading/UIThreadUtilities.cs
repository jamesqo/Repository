using System;
using Android.OS;
using Debug = System.Diagnostics.Debug;

namespace Repository.Internal.Threading
{
    internal static class UIThreadUtilities
    {
        private static Looper MainLooper { get; } = Looper.MainLooper;

        /// <summary>
        /// The <see cref="Handler"/> that corresponds to the UI thread.
        /// May be used to post events to the UI thread.
        /// </summary>
        private static Handler UIThreadHandler { get; } = new Handler(MainLooper);

        private static bool IsRunningOnUIThread => Looper.MyLooper() == MainLooper;

        public static void Post(Action action) => UIThreadHandler.Post(action);

        // This awaitable is different from Task.Yield() in that:
        // - It must be run from the UI threaad.
        // - It posts the continuation to the back of the queue, and thus will not prioritize
        //   it over input/rendering work. (See: https://developer.xamarin.com/api/member/System.Threading.Tasks.Task.Yield()/)
        // This makes it ideal for keeping the UI responsive while doing work on the UI thread.
        public static UIThreadYieldAwaitable Yield()
        {
            Debug.Assert(IsRunningOnUIThread);
            return new UIThreadYieldAwaitable();
        }
    }
}
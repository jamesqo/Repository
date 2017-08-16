using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Repository.Internal.Threading
{
    internal static class ThreadingUtilities
    {
        // TODO: Shouldn't this be disposed?
        public static Handler UIThreadHandler { get; } = new Handler(Looper.MainLooper);
    }
}
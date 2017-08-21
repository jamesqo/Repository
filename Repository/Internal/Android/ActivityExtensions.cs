using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Views;
using Repository.Common.Validation;

namespace Repository.Internal.Android
{
    internal static class ActivityExtensions
    {
        public static void HideActionBar(this Activity activity)
        {
            Verify.NotNull(activity, nameof(activity));

            activity.RequestWindowFeature(WindowFeatures.NoTitle);
            activity.Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
        }
    }
}
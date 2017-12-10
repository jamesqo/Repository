using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Views;
using Repository.Common.Validation;
using Debug = System.Diagnostics.Debug;

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

        // Copied from https://stackoverflow.com/a/8450316/4077294
        public static void LockScreenOrientation(this Activity activity)
        {
            Verify.NotNull(activity, nameof(activity));

            var orientation = activity.Resources.Configuration.Orientation;
            if (orientation == Orientation.Portrait)
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.Froyo)
                {
                    activity.RequestedOrientation = ScreenOrientation.Portrait;
                }
                else
                {
                    var rotation = activity.WindowManager.DefaultDisplay.Rotation;
                    if (rotation == SurfaceOrientation.Rotation90 || rotation == SurfaceOrientation.Rotation180)
                    {
                        activity.RequestedOrientation = ScreenOrientation.ReversePortrait;
                    }
                    else
                    {
                        activity.RequestedOrientation = ScreenOrientation.Portrait;
                    }
                }
            }
            else
            {
                Debug.Assert(orientation == Orientation.Landscape);
                if (Build.VERSION.SdkInt < BuildVersionCodes.Froyo)
                {
                    activity.RequestedOrientation = ScreenOrientation.Landscape;
                }
                else
                {
                    var rotation = activity.WindowManager.DefaultDisplay.Rotation;
                    if (rotation == SurfaceOrientation.Rotation0 || rotation == SurfaceOrientation.Rotation90)
                    {
                        activity.RequestedOrientation = ScreenOrientation.Landscape;
                    }
                    else
                    {
                        activity.RequestedOrientation = ScreenOrientation.ReverseLandscape;
                    }
                }
            }
        }
    }
}
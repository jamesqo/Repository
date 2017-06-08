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

namespace Repository
{
    [Activity]
    public class EditFileActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            HideActionBar();
            SetContentView(Resource.Layout.EditFile);
        }

        private void HideActionBar()
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
        }
    }
}
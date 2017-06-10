using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Content;
using Repository.Internal;
using System.Diagnostics;

namespace Repository
{
    [Activity(Label = Strings.AppName, MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Button _getStartedButton;

        protected override void OnCreate(Bundle bundle)
        {
            void CacheViews()
            {
                _getStartedButton = FindViewById<Button>(Resource.Id.GetStartedButton);
            }

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            CacheViews();

            _getStartedButton.Click += GetStartedButton_Click;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = ((Exception)e.ExceptionObject).GetBaseException();
            System.Diagnostics.Debug.WriteLine(exception);

            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        private void GetStartedButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(ChooseProviderActivity));
            StartActivity(intent);
        }
    }
}


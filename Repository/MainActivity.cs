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
            SetupApp();

            void CacheViews()
            {
                _getStartedButton = FindViewById<Button>(Resource.Id.GetStartedButton);
            }

            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            CacheViews();

            _getStartedButton.Click += GetStartedButton_Click;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = ((Exception)e.ExceptionObject).GetBaseException();
            System.Diagnostics.Debug.WriteLine(exception);

            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        private void GetStartedButton_Click(object sender, EventArgs e) => StartChooseProvider();

        private static void SetupApp()
        {
            SetupAppDomain();
            SetupDebug();
        }

        private static void SetupAppDomain()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private static void SetupDebug()
        {
            System.Diagnostics.Debug.Listeners.Add(new DebugListener());
        }

        private void StartChooseProvider()
        {
            var intent = new Intent(this, typeof(ChooseProviderActivity));
            StartActivity(intent);
        }
    }
}


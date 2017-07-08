using System;
using System.Diagnostics;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Repository.Internal;
using Repository.Internal.Editor;
using Debug = System.Diagnostics.Debug;

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
            Debug.WriteLine(exception);

            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        private void GetStartedButton_Click(object sender, EventArgs e) => StartChooseProvider();

        private void SetupApp()
        {
            SetupAppDomain();
            SetupDebug();
            SetupTypefaces();
        }

        private static void SetupAppDomain()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private static void SetupDebug() => Debug.Listeners.Add(new DebugListener());

        private void SetupTypefaces() => Typefaces.Initialize(Assets);

        private void StartChooseProvider()
        {
            var intent = new Intent(this, typeof(ChooseProviderActivity));
            StartActivity(intent);
        }
    }
}


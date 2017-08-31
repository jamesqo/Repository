using System;
using System.Diagnostics;
using System.Reflection;
using Android.App;
using Android.OS;
using Xamarin.Android.NUnitLite;
using Debug = System.Diagnostics.Debug;

namespace Repository.Editor.Android.UnitTests
{
    [Activity(Label = "Repository.Editor.Android.UnitTests", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : TestSuiteActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            SetupAppDomain();

            AddTest(Assembly.GetExecutingAssembly());

            base.OnCreate(bundle);
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

        private static void SetupAppDomain()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }
    }
}

using System.Reflection;
using Android.App;
using Android.OS;
using Xamarin.Android.NUnitLite;

namespace Repository.Editor.Android.UnitTests
{
    [Activity(Label = "Repository.Editor.Android.UnitTests", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : TestSuiteActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            AddTest(Assembly.GetExecutingAssembly());

            base.OnCreate(bundle);
        }
    }
}

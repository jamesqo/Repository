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
using Repository.Internal;

namespace Repository
{
    [Activity(Label = "Choose a Provider")]
    public class ChooseProviderActivity : Activity
    {
        private Button _githubButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ChooseProvider);

            _githubButton = FindViewById<Button>(Resource.Id.GitHubButton);
            _githubButton.Click += GitHubButton_Click;
        }

        private void GitHubButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(SignInActivity));
            intent.PutExtra(Strings.SignIn_Url, )
            StartActivity(intent);
        }
    }
}
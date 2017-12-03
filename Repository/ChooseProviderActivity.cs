using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Octokit;
using Repository.Internal;
using Repository.Internal.Android;
using Activity = Android.App.Activity;

namespace Repository
{
    [Activity(Label = "@string/Label_ChooseProvider", Name = Strings.Name_ChooseProvider)]
    public class ChooseProviderActivity : Activity
    {
        private Button _githubButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            void CacheViews()
            {
                _githubButton = FindViewById<Button>(Resource.Id.ChooseProvider_GitHubButton);
            }

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ChooseProvider);
            CacheViews();

            _githubButton.Click += GitHubButton_Click;
        }

        private void GitHubButton_Click(object sender, EventArgs e)
        {
            var token = ReadAccessToken(Strings.SPKey_AccessTokens_GitHubAccessToken);
            if (token != null)
            {
                SkipSignIn(token);
                return;
            }

            StartSignIn(url: GetGitHubLoginUrl(), callbackUrl: "https://www.google.com");
        }

        private static string GetGitHubLoginUrl()
        {
            var request = new OauthLoginRequest(Creds.ClientId)
            {
                Scopes = { "repo" }
            };
            return GitHub.Client.Oauth.GetGitHubLoginUrl(request).ToString();
        }

        private string ReadAccessToken(string key)
        {
            var prefs = ApplicationContext.GetSharedPreferences(Strings.SPFile_AccessTokens);
            return prefs.GetString(key, defValue: null);
        }

        private void StartSignIn(string url, string callbackUrl)
        {
            var intent = new Intent(this, typeof(SignInActivity));
            intent.PutExtra(Strings.Extra_SignIn_Url, url);
            intent.PutExtra(Strings.Extra_SignIn_CallbackUrl, callbackUrl);
            StartActivity(intent);
        }

        private void SkipSignIn(string token)
        {
            var intent = new Intent(this, typeof(ChooseRepoActivity));
            GitHub.Client.Credentials = new Credentials(token);
            StartActivity(intent);
        }
    }
}
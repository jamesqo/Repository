using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Octokit;
using Repository.Internal;
using Repository.Internal.Android;
using static Repository.Common.Verify;
using Activity = Android.App.Activity;
using Debug = System.Diagnostics.Debug;

namespace Repository
{
    [Activity]
    public class SignInActivity : Activity
    {
        private class LoginSuccessListener : WebViewClient
        {
            private readonly SignInActivity _activity;
            private readonly string _callbackDomain;

            internal LoginSuccessListener(SignInActivity activity, string callbackDomain)
            {
                _activity = NotNull(activity, nameof(activity));
                _callbackDomain = NotNullOrEmpty(callbackDomain, nameof(callbackDomain));
            }

            public override void OnPageFinished(WebView view, string url)
            {
                if (url.Contains(_callbackDomain))
                {
                    var queryParameters = UrlUtilities.ParseQueryParameters(url);
                    string code = queryParameters["code"];

                    _activity.HandleSessionCode(code);
                }
            }
        }

        private WebView _signInWebView;

        private string _url;
        private string _callbackDomain;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Debug.Assert(
                GitHub.Client.Credentials.AuthenticationType != AuthenticationType.Oauth,
                "The point of being here is to get an OAuth access token.");

            void CacheViews()
            {
                _signInWebView = FindViewById<WebView>(Resource.Id.SignInWebView);
            }

            void CacheParameters()
            {
                _url = NotNullOrEmpty(Intent.Extras.GetString(Strings.Extra_SignIn_Url));
                _callbackDomain = NotNullOrEmpty(Intent.Extras.GetString(Strings.Extra_SignIn_CallbackDomain));
            }

            base.OnCreate(savedInstanceState);

            this.HideActionBar();

            SetContentView(Resource.Layout.SignIn);
            CacheViews();
            CacheParameters();

            SetupWebView();
        }

        private async void HandleSessionCode(string code)
        {
            var token = await RequestAccessToken(code);
            Argument(WriteAccessToken(key: Strings.SPKey_AccessTokens_GitHubAccessToken, token: token));
            GitHub.Client.Credentials = new Credentials(token);

            StartChooseRepo();
        }

        private static async Task<string> RequestAccessToken(string code)
        {
            var request = new OauthTokenRequest(Creds.ClientId, Creds.ClientSecret, code);
            var oauthToken = await GitHub.Client.Oauth.CreateAccessToken(request);
            return oauthToken.AccessToken;
        }

        private void SetupWebView()
        {
            // GitHub needs JS enabled to un-grey the authorization button
            _signInWebView.Settings.JavaScriptEnabled = true;
            _signInWebView.LoadUrl(_url);
            _signInWebView.SetWebViewClient(new LoginSuccessListener(this, _callbackDomain));
        }

        private void StartChooseRepo()
        {
            var intent = new Intent(this, typeof(ChooseRepoActivity));
            StartActivity(intent);
        }

        private bool WriteAccessToken(string key, string token)
        {
            var prefs = ApplicationContext.GetSharedPreferences(Strings.SPFile_AccessTokens);
            var editor = prefs.Edit();
            editor.PutString(key, token);
            return editor.Commit();
        }
    }
}
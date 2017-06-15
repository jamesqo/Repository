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
using Repository.Internal;
using static Repository.Common.Verify;
using static System.Diagnostics.Debug;

namespace Repository
{
    [Activity(Label = "Sign In")]
    public class SignInActivity : Activity
    {
        private sealed class LoginSuccessListener : WebViewClient
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
                    Assert(queryParameters.Count == 1);
                    string code = queryParameters["code"];

                    _activity.OnSessionCodeReceived(code);
                }
            }
        }

        private WebView _signInWebView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            void CacheViews()
            {
                _signInWebView = FindViewById<WebView>(Resource.Id.SignInWebView);
            }

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SignIn);
            CacheViews();

            // GitHub needs JS enabled to un-grey the authorization button
            _signInWebView.Settings.JavaScriptEnabled = true;

            var url = NotNullOrEmpty(Intent.Extras.GetString(Strings.SignIn_Url));
            _signInWebView.LoadUrl(url);

            var callbackDomain = NotNullOrEmpty(Intent.Extras.GetString(Strings.SignIn_CallbackDomain));
            _signInWebView.SetWebViewClient(new LoginSuccessListener(this, callbackDomain));
        }

        private async void OnSessionCodeReceived(string code)
        {
            GitHub.Client.Credentials = await GetCredentials(code);

            var intent = new Intent(this, typeof(ChooseRepositoryActivity));
            StartActivity(intent);
        }

        private static async Task<Octokit.Credentials> GetCredentials(string code)
        {
            var request = new Octokit.OauthTokenRequest(Creds.ClientId, Creds.ClientSecret, code);
            var oauthToken = await GitHub.Client.Oauth.CreateAccessToken(request);
            return new Octokit.Credentials(oauthToken.AccessToken);
        }
    }
}
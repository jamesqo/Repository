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
using static Repository.Internal.Verify;

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
                _activity = NotNull(activity);
                _callbackDomain = NotNull(callbackDomain);
            }

            public override void OnPageFinished(WebView view, string url)
            {
                if (url.Contains(_callbackDomain))
                {
                    var queryParameters = UrlUtilities.ParseQueryParameters(url);
                    System.Diagnostics.Debug.Assert(queryParameters.Count == 1);
                    string code = queryParameters["code"];

                    _activity.OnSessionCodeReceived(code);
                }
            }
        }

        private WebView _signInWebView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SignIn);

            _signInWebView = FindViewById<WebView>(Resource.Id.SignInWebView);
            // GitHub needs JS enabled to un-grey the authorization button
            _signInWebView.Settings.JavaScriptEnabled = true;

            var url = NotNull(Intent.Extras.GetString(Strings.SignIn_Url));
            _signInWebView.LoadUrl(url);

            var callbackDomain = NotNull(Intent.Extras.GetString(Strings.SignIn_CallbackDomain));
            _signInWebView.SetWebViewClient(new LoginSuccessListener(this, callbackDomain));
        }

        private void OnAccessTokenReceived(string accessToken)
        {
            var intent = new Intent(this, typeof(FileViewActivity));
            intent.PutExtra(Strings.FileView_AccessToken, accessToken);
            StartActivity(intent);
        }

        private async void OnSessionCodeReceived(string code)
        {
            string accessToken = await RetrieveAccessToken(code);
            OnAccessTokenReceived(accessToken);
        }

        private static async Task<string> RetrieveAccessToken(string code)
        {
            var request = new Octokit.OauthTokenRequest(Creds.ClientId, Creds.ClientSecret, code);
            var oauthToken = await GitHub.Client.Oauth.CreateAccessToken(request);
            return oauthToken.AccessToken;
        }
    }
}
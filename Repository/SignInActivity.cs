using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            private readonly string _callbackUrl;

            internal LoginSuccessListener(string callbackUrl)
            {
                _callbackUrl = NotNull(callbackUrl);
            }

            public override void OnPageFinished(WebView view, string url)
            {
                if (url.StartsWith(_callbackUrl, StringComparison.Ordinal))
                {
                    // TODO: Start some activity?
                }
            }
        }

        private WebView _signInWebView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SignIn);

            _signInWebView = FindViewById<WebView>(Resource.Id.SignInWebView);

            var url = NotNull(Intent.Extras.GetString(Strings.SignIn_Url));
            _signInWebView.LoadUrl(url);

            var callbackUrl = NotNull(Intent.Extras.GetString(Strings.SignIn_CallbackUrl));
            _signInWebView.SetWebViewClient(new LoginSuccessListener(callbackUrl));
        }
    }
}
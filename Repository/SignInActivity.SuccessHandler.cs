using Android.Webkit;
using Repository.Common;
using Repository.Internal;
using static Repository.Common.Verify;
using Activity = Android.App.Activity;

namespace Repository
{
    public partial class SignInActivity : Activity
    {
        private class SuccessHandler : WebViewClient
        {
            private readonly SignInActivity _activity;
            private readonly string _callbackUrl;

            internal SuccessHandler(SignInActivity activity, string callbackUrl)
            {
                _activity = NotNull(activity, nameof(activity));
                _callbackUrl = NotNullOrEmpty(callbackUrl, nameof(callbackUrl));
            }

            public override void OnPageFinished(WebView view, string url)
            {
                if (url.StartsWithOrdinal(_callbackUrl))
                {
                    var parameters = UrlUtilities.ParseQueryParameters(url);
                    string code = parameters["code"];
                    _activity.HandleSessionCode(code);
                }
            }
        }
    }
}
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Webkit;
using Octokit;
using Repository.Internal;
using Repository.Internal.Android;
using static Repository.Common.Verify;
using Activity = Android.App.Activity;
using Debug = System.Diagnostics.Debug;

namespace Repository
{
    [Activity]
    public partial class SignInActivity : Activity
    {
        private WebView _signInWebView;

        private string _url;
        private string _callbackUrl;

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
                _callbackUrl = NotNullOrEmpty(Intent.Extras.GetString(Strings.Extra_SignIn_CallbackUrl));
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

            StartChooseRepo(token);
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
            _signInWebView.SetWebViewClient(new SuccessHandler(this, _callbackUrl));
        }

        private void StartChooseRepo(string token)
        {
            var intent = new Intent(this, typeof(ChooseRepoActivity));
            GitHub.Client.Credentials = new Credentials(token);
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
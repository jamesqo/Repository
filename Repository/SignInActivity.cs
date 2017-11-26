using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Webkit;
using Octokit;
using Repository.Common.Validation;
using Repository.Internal;
using Repository.Internal.Android;
using Activity = Android.App.Activity;

namespace Repository
{
    [Activity(Name = "com.bluejay.repository.SignInActivity")]
    public partial class SignInActivity : Activity
    {
        private WebView _webView;

        private string _url;
        private string _callbackUrl;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Verify.ValidState(
                GitHub.Client.Credentials.AuthenticationType != AuthenticationType.Oauth,
                "The point of being here is to get an OAuth access token.");

            void CacheViews()
            {
                _webView = FindViewById<WebView>(Resource.Id.SignIn_WebView);
            }

            void CacheParameters()
            {
                _url = Intent.Extras.GetString(Strings.Extra_SignIn_Url).NotNullOrEmpty();
                _callbackUrl = Intent.Extras.GetString(Strings.Extra_SignIn_CallbackUrl).NotNullOrEmpty();
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
            WriteAccessToken(key: Strings.SPKey_AccessTokens_GitHubAccessToken, token: token);

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
            _webView.Settings.JavaScriptEnabled = true;
            _webView.LoadUrl(_url);
            _webView.SetWebViewClient(new SuccessHandler(this, _callbackUrl));
        }

        private void StartChooseRepo(string token)
        {
            var intent = new Intent(this, typeof(ChooseRepoActivity));
            GitHub.Client.Credentials = new Credentials(token);
            StartActivity(intent);
        }

        private void WriteAccessToken(string key, string token)
        {
            string fileName = Strings.SPFile_AccessTokens;
            var prefs = ApplicationContext.GetSharedPreferences(fileName);
            var editor = prefs.Edit();
            editor.PutString(key, token);
            bool committed = editor.Commit();
            Verify.ValidState(committed, Strings.SPFile_CommitFailed_Message(fileName));
        }
    }
}
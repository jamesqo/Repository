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

namespace Repository.Internal
{
    internal static class Strings
    {
        public const string AppName = "Repository";

        public const string Extra_BrowseFiles_RepoId = nameof(Extra_BrowseFiles_RepoId);

        public const string Extra_EditFile_Path = nameof(Extra_EditFile_Path);

        public const string Extra_SignIn_CallbackUrl = nameof(Extra_SignIn_CallbackUrl);

        public const string Extra_SignIn_Url = nameof(Extra_SignIn_Url);

        public const string Label_ChooseProvider = "Choose a Provider";

        public const string Label_ChooseRepo = "Choose a Repository";

        public const string SPFile_AccessTokens = nameof(SPFile_AccessTokens);

        public const string SPKey_AccessTokens_GitHubAccessToken = nameof(SPKey_AccessTokens_GitHubAccessToken);
    }
}
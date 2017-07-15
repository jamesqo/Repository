using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository.Internal
{
    /// <summary>
    /// Stores strings that are not intended for display to the user.
    /// </summary>
    /// <remarks>
    /// Strings that are intended for display to the user should go in Strings.xml.
    /// </remarks>
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
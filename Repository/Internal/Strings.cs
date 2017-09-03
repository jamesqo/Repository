﻿namespace Repository.Internal
{
    /// <summary>
    /// Stores strings that are not intended for display to the user.
    /// </summary>
    /// <remarks>
    /// Strings that are intended for display to the user should go in Strings.xml.
    /// </remarks>
    internal static class Strings
    {
        // This is not to be used in a string displayed to the user. Use Resource.String.app_name instead.
        public const string AppName = "Repository";

        public const string Asset_Font_Inconsolata = "fonts/Inconsolata.ttf";

        public const string Extra_BrowseFiles_RepoId = nameof(Extra_BrowseFiles_RepoId);

        public const string Extra_EditFile_Path = nameof(Extra_EditFile_Path);

        public const string Extra_SignIn_CallbackUrl = nameof(Extra_SignIn_CallbackUrl);

        public const string Extra_SignIn_Url = nameof(Extra_SignIn_Url);

        public const string SPFile_AccessTokens = nameof(SPFile_AccessTokens);

        public static string SPFile_CommitFailed_Message(string fileName)
        {
            return $"Unable to commit to SharedPreferences file {fileName}";
        }

        public const string SPKey_AccessTokens_GitHubAccessToken = nameof(SPKey_AccessTokens_GitHubAccessToken);
    }
}
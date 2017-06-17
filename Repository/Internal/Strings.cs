﻿using System;
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

        public const string Extra_EditFile_Content = nameof(Extra_EditFile_Content);

        public const string Extra_EditFile_Path = nameof(Extra_EditFile_Path);

        public const string Extra_SignIn_Url = nameof(Extra_SignIn_Url);

        public const string Extra_SignIn_CallbackDomain = nameof(Extra_SignIn_CallbackDomain);

        public const string SPFile_AccessTokens = nameof(SPFile_AccessTokens);

        public const string SPFile_EditorContent = nameof(SPFile_EditorContent);

        public const string SPKey_AccessTokens_GitHubAccessToken = nameof(SPKey_AccessTokens_GitHubAccessToken);

        public const string SPKey_EditorContent_Value = nameof(SPKey_EditorContent_Value);
    }
}
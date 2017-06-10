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
using Repository.Internal;

namespace Repository
{
    [Activity(Label = "Choose a Provider")]
    public class ChooseProviderActivity : Activity
    {
        private static string CallbackDomain { get; } = "google.com";

        private Button _githubButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            void CacheViews()
            {
                _githubButton = FindViewById<Button>(Resource.Id.GitHubButton);
            }

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ChooseProvider);
            CacheViews();

            _githubButton.Click += GitHubButton_Click;
        }

        private void GitHubButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(SignInActivity));
            intent.PutExtra(Strings.SignIn_Url, GetGitHubLoginUrl());
            intent.PutExtra(Strings.SignIn_CallbackDomain, CallbackDomain);
            StartActivity(intent);
        }

        private static string GetGitHubLoginUrl()
        {
            var request = new Octokit.OauthLoginRequest(Creds.ClientId)
            {
                Scopes = { "repo" }
            };
            return GitHub.Client.Oauth.GetGitHubLoginUrl(request).ToString();
        }
    }
}
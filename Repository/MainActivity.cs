﻿using System;
using System.Diagnostics;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Repository.Internal;
using Repository.Internal.Diagnostics;
using Debug = System.Diagnostics.Debug;

namespace Repository
{
    [Activity(Icon = "@drawable/icon", Label = "@string/app_name", MainLauncher = true, Name = Strings.Name_Main)]
    public class MainActivity : Activity
    {
        private Button _startButton;

        protected override void OnCreate(Bundle bundle)
        {
            SetupApp();

            void CacheViews()
            {
                _startButton = FindViewById<Button>(Resource.Id.Main_StartButton);
            }

            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            CacheViews();

            _startButton.Click += StartButton_Click;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = ((Exception)e.ExceptionObject).GetBaseException();
            Debug.WriteLine(exception);

            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        private void SetupApp()
        {
            SetupAppDomain();
            SetupDebug();
        }

        private static void SetupAppDomain()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private static void SetupDebug() => Debug.Listeners.Add(new DebugListener());

        private void StartButton_Click(object sender, EventArgs e) => StartChooseProvider();

        private void StartChooseProvider()
        {
            var intent = new Intent(this, typeof(ChooseProviderActivity));
            StartActivity(intent);
        }
    }
}


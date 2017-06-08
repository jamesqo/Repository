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
        public static string SignIn_Url { get; } = nameof(SignIn_Url);

        public static string SignIn_CallbackUrl { get; } = nameof(SignIn_CallbackUrl);
    }
}
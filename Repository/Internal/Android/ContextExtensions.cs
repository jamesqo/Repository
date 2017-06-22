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

namespace Repository.Internal.Android
{
    internal static class ContextExtensions
    {
        public static ISharedPreferences GetSharedPreferences(this Context context, string name)
        {
            return context.GetSharedPreferences(name, FileCreationMode.Private);
        }
    }
}
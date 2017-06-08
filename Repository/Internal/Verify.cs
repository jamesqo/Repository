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
    internal static class Verify
    {
        public static T NotNull<T>(T argument) where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            return argument;
        }
    }
}
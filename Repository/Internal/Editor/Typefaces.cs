using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content.Res;
using Android.Graphics;
using Repository.Common;

namespace Repository.Internal.Editor
{
    internal static class Typefaces
    {
        public static Typeface Inconsolata { get; private set; }

        internal static void Initialize(AssetManager assets)
        {
            Verify.NotNull(assets, nameof(assets));

            Inconsolata = Typeface.CreateFromAsset(assets, "fonts/Inconsolata.ttf");
        }
    }
}
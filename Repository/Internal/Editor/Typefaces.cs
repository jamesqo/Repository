using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Android.Content.Res;
using Android.Graphics;
using Repository.Common;

namespace Repository.Internal.Editor
{
    // TODO: I don't like this. Either find a less hackish way to load the typefaces statically,
    // or make this an instance class like TypefaceManager.
    internal static class Typefaces
    {
        private static Typeface s_inconsolata;

        public static Typeface Inconsolata => CheckInitialized(s_inconsolata);

        internal static void Initialize(AssetManager assets)
        {
            Verify.NotNull(assets, nameof(assets));

            s_inconsolata = Typeface.CreateFromAsset(assets, "fonts/Inconsolata.ttf");
        }

        private static Typeface CheckInitialized(Typeface typeface)
        {
            Verify.State(typeface != null, "Typefaces weren't initialized yet.");
            return typeface;
        }
    }
}
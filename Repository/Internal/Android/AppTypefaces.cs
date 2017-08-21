using System.Collections.Generic;
using Android.Content.Res;
using Android.Graphics;
using Repository.Internal.Collections;

namespace Repository.Internal.Android
{
    internal class AppTypefaces
    {
        private static Dictionary<AssetManager, AppTypefaces> Instances { get; } =
            new Dictionary<AssetManager, AppTypefaces>(ReferenceEqualityComparer.Instance);

        public static AppTypefaces GetInstance(AssetManager assets)
        {
            return Instances.GetOrAdd(assets, a => new AppTypefaces(a));
        }

        private AppTypefaces(AssetManager assets)
        {
            Inconsolata = Typeface.CreateFromAsset(assets, "fonts/Inconsolata.ttf");
        }

        public Typeface Inconsolata { get; }
    }
}
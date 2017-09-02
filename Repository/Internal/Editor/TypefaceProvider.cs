using System.Collections.Generic;
using Android.Content.Res;
using Android.Graphics;
using Repository.Common.Collections;
using Repository.Editor.Android;
using Repository.Internal.Collections;

namespace Repository.Internal.Editor
{
    internal class TypefaceProvider : ITypefaceProvider
    {
        private static Dictionary<AssetManager, TypefaceProvider> Instances { get; } =
            new Dictionary<AssetManager, TypefaceProvider>(ReferenceEqualityComparer.Instance);

        public static TypefaceProvider GetInstance(AssetManager assets)
        {
            return Instances.GetOrAdd(assets, a => new TypefaceProvider(a));
        }

        private TypefaceProvider(AssetManager assets)
        {
            Inconsolata = Typeface.CreateFromAsset(assets, Strings.Asset_Font_Inconsolata);
        }

        public Typeface Inconsolata { get; }
    }
}
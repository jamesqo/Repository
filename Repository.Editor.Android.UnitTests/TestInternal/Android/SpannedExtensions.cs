using System.Collections.Generic;
using System.Linq;
using Android.Text;
using Java.Lang;
using Repository.Common.Validation;

namespace Repository.Editor.Android.UnitTests.TestInternal.Android
{
    internal static class SpannedExtensions
    {
        public static IEnumerable<T> GetSpans<T>(this ISpanned spanned)
               where T : Java.Lang.Object
        {
            Verify.NotNull(spanned, nameof(spanned));

            return spanned.GetSpans(0, spanned.Length(), Class.FromType(typeof(T))).Cast<T>();
        }
    }
}

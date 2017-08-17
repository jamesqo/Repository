using Android.Text;
using Repository.Common;
using JavaObject = Java.Lang.Object;

namespace Repository.Internal.Android
{
    internal static class SpannableExtensions
    {
        public static void SetSpan(this ISpannable spannable, JavaObject what)
        {
            Verify.NotNull(spannable, nameof(spannable));

            spannable.SetSpan(what, 0, spannable.Length(), SpanTypes.InclusiveExclusive);
        }
    }
}
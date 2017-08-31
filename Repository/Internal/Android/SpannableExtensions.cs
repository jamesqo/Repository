using Android.Text;
using Repository.Common.Validation;
using JavaObject = Java.Lang.Object;

namespace Repository.Internal.Android
{
    // TODO: Consolidate with SpannedExtensions in a Repository.Common.Android lib.
    internal static class SpannableExtensions
    {
        /// <summary>
        /// Sets a span across the entirety of an <see cref="ISpannable"/>.
        /// </summary>
        /// <param name="spannable">The spannable.</param>
        /// <param name="what">The span.</param>
        public static void SetSpan(this ISpannable spannable, JavaObject what)
        {
            Verify.NotNull(spannable, nameof(spannable));

            spannable.SetSpan(what, 0, spannable.Length(), SpanTypes.InclusiveExclusive);
        }
    }
}
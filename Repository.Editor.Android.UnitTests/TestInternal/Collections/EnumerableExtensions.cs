using System.Collections.Generic;
using Repository.Common.Validation;

namespace Repository.Editor.Android.UnitTests.TestInternal.Collections
{
    internal static class EnumerableExtensions
    {
        public static bool StartsWith<T>(this IEnumerable<T> source, IEnumerable<T> prefix, IEqualityComparer<T> comparer = null)
        {
            Verify.NotNull(source, nameof(source));
            Verify.NotNull(prefix, nameof(prefix));

            comparer = comparer ?? EqualityComparer<T>.Default;

            using (var sourceEnumerator = source.GetEnumerator())
            using (var prefixEnumerator = prefix.GetEnumerator())
            {
                while (true)
                {
                    if (!sourceEnumerator.MoveNext())
                    {
                        return !prefixEnumerator.MoveNext();
                    }

                    if (!prefixEnumerator.MoveNext())
                    {
                        return true;
                    }

                    if (!comparer.Equals(sourceEnumerator.Current, prefixEnumerator.Current))
                    {
                        return false;
                    }
                }
            }
        }
    }
}

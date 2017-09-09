using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Repository.Common.Validation;

namespace Repository.Common.Collections
{
    public static class EnumerableExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> source, IEnumerable<T> subEnumerable, IEqualityComparer<T> comparer = null)
        {
            bool StartsWith(T[] prefix, T[] sourceArray, int index)
            {
                for (int i = 0; i < prefix.Length; i++)
                {
                    if (!comparer.Equals(prefix[i], sourceArray[index + i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            Verify.NotNull(source, nameof(source));
            Verify.NotNullOrEmpty(subEnumerable, nameof(subEnumerable));

            var array = source.ToArray();
            var subArray = subEnumerable.ToArray();
            int diff = array.Length - subArray.Length;

            for (int i = 0; i <= diff; i++)
            {
                if (StartsWith(subArray, array, i))
                {
                    return i;
                }
            }

            return -1;
        }

        public static IEnumerable<T> Insert<T>(this IEnumerable<T> source, int index, T item)
        {
            Verify.NotNull(source, nameof(source));

            var list = source.ToList();
            list.Insert(index, item);
            return list;
        }

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

        public static ReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T> source)
        {
            return new ReadOnlyCollection<T>(source.ToList());
        }
    }
}
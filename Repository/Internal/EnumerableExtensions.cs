using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Repository.Internal
{
    internal static class EnumerableExtensions
    {
        public static LazyList<T> ToLazyList<T>(this IEnumerable<T> source) => LazyList<T>.Create(source);

        public static ReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T> source)
        {
            return new ReadOnlyCollection<T>(source.ToList());
        }
    }
}
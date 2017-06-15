using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Repository.EditorServices.Internal
{
    internal static class ImmutableArrayExtensions
    {
        public static ImmutableSpan<T> Slice<T>(this ImmutableArray<T> array, int index, int count)
        {
            return ImmutableSpan<T>.Create(array, index, count);
        }
    }
}
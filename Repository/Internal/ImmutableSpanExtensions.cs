using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository.Internal
{
    internal static class ImmutableSpanExtensions
    {
        public static bool StartsWithReferences<T>(this ImmutableSpan<T> span, ImmutableSpan<T> other)
            where T : class
        {
            if (span.Count < other.Count)
            {
                return false;
            }

            for (int i = 0; i < other.Count; i++)
            {
                if (!ReferenceEquals(span[i], other[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
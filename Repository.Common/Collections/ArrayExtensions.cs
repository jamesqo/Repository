using System;
using Repository.Common.Validation;

namespace Repository.Common.Collections
{
    public static class ArrayExtensions
    {
        public static ArraySegment<T> Slice<T>(this T[] array, int index)
        {
            Verify.NotNull(array, nameof(array));
            Verify.InRange(index >= 0 && index <= array.Length, nameof(index));

            return new ArraySegment<T>(array, index, array.Length - index);
        }
    }
}

using System.Collections;

namespace Repository.Common.Validation
{
    public static class ValidationExtensions
    {
        public static T NotNull<T>(this T obj)
            where T : class
        {
            Verify.NotNull(obj, argumentName: null);
            return obj;
        }

        public static TEnumerable NotNullOrEmpty<TEnumerable>(this TEnumerable obj)
            where TEnumerable : class, IEnumerable
        {
            Verify.NotNullOrEmpty(obj, argumentName: null);
            return obj;
        }
    }
}

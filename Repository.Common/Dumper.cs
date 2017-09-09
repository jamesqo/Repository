using System.Collections.Generic;
using Repository.Common.Validation;

namespace Repository.Common
{
    public static class Dumper
    {
        public static string ToString<T>(IEnumerable<T> enumerable)
        {
            Verify.NotNull(enumerable, nameof(enumerable));

            return '[' + string.Join(", ", enumerable) + ']';
        }
    }
}
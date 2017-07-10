using System;

namespace Repository.Common
{
    public static class StringExtensions
    {
        public static bool EndsWithOrdinal(this string text, string value)
        {
            Verify.NotNull(text, nameof(text));
            Verify.NotNull(value, nameof(value));

            return text.EndsWith(value, StringComparison.Ordinal);
        }

        public static bool StartsWithOrdinal(this string text, string value)
        {
            Verify.NotNull(text, nameof(text));
            Verify.NotNull(value, nameof(value));

            return text.StartsWith(value, StringComparison.Ordinal);
        }
    }
}
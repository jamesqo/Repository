using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Repository.Common;

namespace Repository.Internal
{
    internal static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFactory)
        {
            Verify.NotNull(dictionary, nameof(dictionary));
            // TODO: Validate `key` too. NotNull won't allow types that aren't guaranteed to be classes though.

            if (!dictionary.TryGetValue(key, out TValue value))
            {
                value = dictionary[key] = valueFactory(key);
            }

            return value;
        }
    }
}
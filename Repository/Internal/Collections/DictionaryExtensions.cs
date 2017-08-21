using System;
using System.Collections.Generic;
using Repository.Common;

namespace Repository.Internal.Collections
{
    internal static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFactory)
        {
            Verify.NotNull(dictionary, nameof(dictionary));
            Verify.NotNullGeneric(key, nameof(key));
            Verify.NotNull(valueFactory, nameof(valueFactory));

            if (!dictionary.TryGetValue(key, out TValue value))
            {
                value = valueFactory(key);
            }

            return value;
        }
    }
}
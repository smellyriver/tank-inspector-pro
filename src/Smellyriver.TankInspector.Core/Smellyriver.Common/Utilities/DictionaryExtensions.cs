using System;
using System.Collections.Generic;

namespace Smellyriver.Utilities
{
    internal static class DictionaryExtensions
    {
        public static TValue GetOrCreate<TDictionary, TKey, TValue>(this TDictionary dictionary, TKey key, Func<TValue> valueFactory)
               where TDictionary : IDictionary<TKey, TValue>
        {
            TValue value;
            if (dictionary.TryGetValue(key, out value))
                return value;
            else
            {
                value = valueFactory();
                dictionary[key] = value;
                return value;
            }
        }

        public static TValue GetOrCreate<TDictionary, TKey, TValue>(this TDictionary dictionary, TKey key, Func<TKey, TValue> valueFactory)
            where TDictionary : IDictionary<TKey, TValue>
        {
            TValue value;
            if (dictionary.TryGetValue(key, out value))
                return value;
            else
            {
                value = valueFactory(key);
                dictionary[key] = value;
                return value;
            }
        }

        public static void GetOrCreate<TDictionary, TKey, TValue>(this TDictionary dictionary, TKey key, out TValue value)
            where TDictionary : IDictionary<TKey, TValue>
            where TValue : new()
        {
            value = dictionary.GetOrCreate(key, () => new TValue());
        }
    }
}

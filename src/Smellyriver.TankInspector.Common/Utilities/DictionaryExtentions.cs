using System;
using System.Collections.Generic;
using System.Linq;

namespace Smellyriver.TankInspector.Common.Utilities
{
    public static class DictionaryExtentions
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
            value = GetOrCreate(dictionary, key, () => new TValue());
        }

        public static void RemoveWhere<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            foreach (var item in dictionary.Where(predicate).ToArray())
                dictionary.Remove(item.Key);
        }

    }
}

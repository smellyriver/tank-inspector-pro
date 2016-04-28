using System;
using System.Collections.Generic;
using System.Linq;

namespace Smellyriver.TankInspector.Common.Utilities
{
    public static class IListExtensions
    {
        public static void RemoveWhere<T>(this IList<T> @this, Func<T, bool> predicate)
        {
            foreach (var item in @this.Where(predicate).ToArray())
                @this.Remove(item);
        }

        public static void AddRange<T>(this IList<T> @this, IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                @this.Add(value);
            }
        }

        public static void AddIfNotNull<T>(this IList<T> @this, T value)
            where T : class
        {
            if (value != null)
                @this.Add(value);
        }

        public static void AddRangeIfNotNull<T>(this IList<T> @this, IEnumerable<T> values)
            where T : class
        {
            foreach (var value in values)
            {
                @this.AddIfNotNull(value);
            }
        }

        public static T GetRandomElement<T>(this IList<T> @this, Random random = null)
        {
            if (random == null)
                random = new Random();

            if (@this == null || @this.Count == 0)
                throw new ArgumentException("array");
            return @this[random.Next(@this.Count)];
        }

        public static void Shuffle<T>(this IList<T> @this, Random random = null)
        {
            if (random == null)
                random = new Random();

            for (int i = @this.Count; i > 1; i--)
            {
                int j = random.Next(i);

                T tmp = @this[j];
                @this[j] = @this[i - 1];
                @this[i - 1] = tmp;
            }
        }
    }
}

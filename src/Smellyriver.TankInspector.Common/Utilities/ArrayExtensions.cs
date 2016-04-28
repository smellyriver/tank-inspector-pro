using System;
using System.Collections.Generic;
using System.Linq;

namespace Smellyriver.TankInspector.Common.Utilities
{
    public static class ArrayExtensions
    {

        public static T GetRandomElement<T>(this T[] array, Random random = null)
        {
            if (random == null)
                random = new Random();

            if (array == null || array.Length == 0)
                throw new ArgumentException("array");
            return array[random.Next(array.Length)];
        }

        public static void Shuffle<T>(this T[] array, Random random = null)
        {
            if (random == null)
                random = new Random();

            for (int i = array.Length; i > 1; i--)
            {
                int j = random.Next(i);

                T tmp = array[j];
                array[j] = array[i - 1];
                array[i - 1] = tmp;
            }
        }

        public static T[] SubArray<T>(this T[] array, int startIndex, int length)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("startIndex");

            if (length < 0)
                return new T[0];

            var result = new T[length];
            Array.Copy(array, startIndex, result, 0, length);
            return result;
        }

        public static T[] SubArray<T>(this T[] array, int length)
        {
            return SubArray<T>(array, 0, length);
        }

        public static int IndexOf<T>(this T[] array, T item)
        {
            for (var i = 0; i < array.Length; ++i)
            {
                if (array[i].Equals(item))
                    return i;
            }

            return -1;
        }

        public static void ForEach(this Array array, Action<Array, int[]> action)
        {
            if (array.LongLength == 0) return;
            ArrayWalker walker = new ArrayWalker(array);
            do action(array, walker.Position);
            while (walker.Step());
        }

        public static T[] Insert<T>(this T[] array, int index, T element)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (index >= array.Length)
                throw new ArgumentOutOfRangeException("index");

            var newArray = new T[array.Length + 1];

            if (index != 0)
                Array.Copy(array, newArray, index);

            newArray[index] = element;

            if (index != array.Length - 1)
                Array.Copy(array, index, newArray, index + 1, array.Length - index);

            return newArray;
        }

        public static T[] Add<T>(this T[] array, T element)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            var newArray = new T[array.Length + 1];
            if (array.Length > 0)
                Array.Copy(array, newArray, array.Length);

            newArray[newArray.Length - 1] = element;

            return newArray;
        }

        public static T[] Remove<T>(this T[] array, T element)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            var result = new List<T>();

            foreach (var item in array)
            {
                if (!item.Equals(element))
                    result.Add(item);
            }

            return result.ToArray();
        }

        public static T[] RemoveWhere<T>(this T[] @this, Func<T, bool> predicate)
        {
            return @this.Except(@this.Where(predicate)).ToArray();
        }

    }
}

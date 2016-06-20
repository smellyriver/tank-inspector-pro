using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smellyriver.Utilities
{
    public static class AotSafeLinqExtensions
    {
        public static int AotSafeMax(this IEnumerable<int> values)
        {
            var max = int.MinValue;
            foreach (var v in values)
                if (v > max)
                    max = v;

            return max;
        }

        public static float AotSafeMax(this IEnumerable<float> values)
        {
            var max = float.MinValue;
            foreach (var v in values)
                if (v > max)
                    max = v;

            return max;
        }

        public static double AotSafeMax(this IEnumerable<double> values)
        {
            var max = double.MinValue;
            foreach (var v in values)
                if (v > max)
                    max = v;

            return max;
        }

        public static int AotSafeMax<T>(this IEnumerable<T> items, Func<T, int> selector)
        {
            var max = int.MinValue;
            foreach (var item in items)
            {
                var value = selector(item);
                if (value > max)
                    max = value;
            }

            return max;
        }

        public static float AotSafeMax<T>(this IEnumerable<T> items, Func<T, float> selector)
        {
            var max = float.MinValue;
            foreach (var item in items)
            {
                var value = selector(item);
                if (value > max)
                    max = value;
            }

            return max;
        }

        public static double AotSafeMax<T>(this IEnumerable<T> items, Func<T, double> selector)
        {
            var max = double.MinValue;
            foreach (var item in items)
            {
                var value = selector(item);
                if (value > max)
                    max = value;
            }

            return max;
        }

        public static int AotSafeMin(this IEnumerable<int> values)
        {
            var min = int.MaxValue;
            foreach (var v in values)
                if (v < min)
                    min = v;

            return min;
        }

        public static float AotSafeMin(this IEnumerable<float> values)
        {
            var min = float.MaxValue;
            foreach (var v in values)
                if (v < min)
                    min = v;

            return min;
        }

        public static double AotSafeMin(this IEnumerable<double> values)
        {
            var min = double.MaxValue;
            foreach (var v in values)
                if (v < min)
                    min = v;

            return min;
        }

        public static int AotSafeMin<T>(this IEnumerable<T> items, Func<T, int> selector)
        {
            var min = int.MaxValue;
            foreach (var item in items)
            {
                var value = selector(item);
                if (value < min)
                    min = value;
            }

            return min;
        }

        public static float AotSafeMin<T>(this IEnumerable<T> items, Func<T, float> selector)
        {
            var min = float.MaxValue;
            foreach (var item in items)
            {
                var value = selector(item);
                if (value < min)
                    min = value;
            }

            return min;
        }

        public static double AotSafeMin<T>(this IEnumerable<T> items, Func<T, double> selector)
        {
            var min = double.MaxValue;
            foreach (var item in items)
            {
                var value = selector(item);
                if (value < min)
                    min = value;
            }

            return min;
        }

        //



        public static int AotSafeAverage(this IEnumerable<int> values)
        {
            var sum = 0;
            var count = 0;

            foreach (var v in values)
            {
                sum += v;
                ++count;
            }

            return sum / count;
        }

        public static float AotSafeAverage(this IEnumerable<float> values)
        {
            var sum = 0f;
            var count = 0;

            foreach (var v in values)
            {
                sum += v;
                ++count;
            }

            return sum / count;
        }

        public static double AotSafeAverage(this IEnumerable<double> values)
        {
            var sum = 0d;
            var count = 0;

            foreach (var v in values)
            {
                sum += v;
                ++count;
            }

            return sum / count;
        }

        public static int AotSafeAverage<T>(this IEnumerable<T> items, Func<T, int> selector)
        {
            var sum = 0;
            var count = 0;

            foreach (var item in items)
            {
                sum += selector(item);
                ++count;
            }

            return sum / count;
        }

        public static float AotSafeAverage<T>(this IEnumerable<T> items, Func<T, float> selector)
        {
            var sum = 0f;
            var count = 0;

            foreach (var item in items)
            {
                sum += selector(item);
                ++count;
            }

            return sum / count;
        }

        public static double AotSafeAverage<T>(this IEnumerable<T> items, Func<T, double> selector)
        {
            var sum = 0d;
            var count = 0;

            foreach (var item in items)
            {
                sum += selector(item);
                ++count;
            }

            return sum / count;
        }
    }
}

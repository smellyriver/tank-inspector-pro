using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Smellyriver.TankInspector.Common.Utilities
{
    public static class LINQExtensions
    {
        public static double Median(this IEnumerable<double> source)
        {
            var count = source.Count();

            if (count == 0)
                throw new InvalidOperationException("Cannot compute median for an empty set.");

            var sortedList = source.OrderBy(n => n);

            var itemIndex = count / 2;

            if (count % 2 == 0) // Even number of items.
                return (sortedList.ElementAt(itemIndex) + sortedList.ElementAt(itemIndex - 1)) / 2;
            else                // Odd number of items.
                return sortedList.ElementAt(itemIndex);
        }

        public static double Median(this IEnumerable<int> source)
        {
            var count = source.Count();

            if (count == 0)
                throw new InvalidOperationException("Cannot compute median for an empty set.");

            var sortedList = source.OrderBy(n => n);

            var itemIndex = count / 2;

            if (count % 2 == 0) // Even number of items.
                return (double)(sortedList.ElementAt(itemIndex) + sortedList.ElementAt(itemIndex - 1)) / 2.0;
            else                // Odd number of items.
                return sortedList.ElementAt(itemIndex);
        }

        public static double Median<T>(this IEnumerable<T> source, Func<T, double> selector)
        {
            return Median(source.Select(selector));
        }

        public static double Median<T>(this IEnumerable<T> source, Func<T, int> selector)
        {
            return Median(source.Select(selector));
        }

        public static bool Majority<T>(this IEnumerable<T> source, Func<T, bool> selector)
        {
            return source.Count(selector) >= source.Count() / 2;
        }

        public static TResult Majority<T, TResult>(this IEnumerable<T> source, Func<T, TResult> selector)
        {
            var counters = new Dictionary<TResult, int>();
            foreach (var item in source)
            {
                var selectResult = selector(item);
                if (!counters.ContainsKey(selectResult))
                    counters.Add(selectResult, 1);
                else
                    ++counters[selectResult];
            }

            var result = default(TResult);
            var maxCounter = 0;
            foreach (var counter in counters)
            {
                if (counter.Value > maxCounter)
                {
                    maxCounter = counter.Value;
                    result = counter.Key;
                }
            }

            return result;
        }

        public static T[] WithMax<T, TSelection>(this IEnumerable<T> source, Func<T, TSelection> selector, IComparer comparer = null)
        {
            if (source == null || !source.Any())
                return new T[0];

            if (comparer == null)
                comparer = Comparer<TSelection>.Default;

            var itemWithMaxValue = source.First();
            var itemsWithMaxValue = new List<T>();
            itemsWithMaxValue.Add(itemWithMaxValue);
            TSelection maxValue = selector(itemWithMaxValue);

            var isFirst = true;

            foreach (var item in source)
            {
                if (isFirst)
                {
                    isFirst = false;
                    continue;
                }

                var value = selector(item);
                var compareResult = comparer.Compare(value, maxValue);
                if (compareResult == 0)
                {
                    itemsWithMaxValue.Add(item);
                }
                else if (compareResult > 0)
                {
                    itemWithMaxValue = item;
                    maxValue = value;
                    itemsWithMaxValue.Clear();
                    itemsWithMaxValue.Add(item);
                }
            }

            return itemsWithMaxValue.ToArray();
        }

        public static T FirstWithMax<T, TSelection>(this IEnumerable<T> source, Func<T, TSelection> selector, IComparer comparer = null)
        {
            if (source == null || !source.Any())
                return default(T);

            if (comparer == null)
                comparer = Comparer<TSelection>.Default;

            var itemWithMaxValue = source.First();
            TSelection maxValue = selector(itemWithMaxValue);

            foreach (var item in source)
            {
                var value = selector(item);
                if (comparer.Compare(value, maxValue) > 0)
                {
                    itemWithMaxValue = item;
                    maxValue = value;
                }
            }

            return itemWithMaxValue;
        }

        public static T[] WithMin<T, TSelection>(this IEnumerable<T> source, Func<T, TSelection> selector, IComparer comparer = null)
        {
            if (source == null || !source.Any())
                return new T[0];

            if (comparer == null)
                comparer = Comparer<TSelection>.Default;

            var itemWithMinValue = source.First();
            var itemsWithMinValue = new List<T>();
            itemsWithMinValue.Add(itemWithMinValue);
            TSelection minValue = selector(itemWithMinValue);

            var isFirst = true;

            foreach (var item in source)
            {
                if (isFirst)
                {
                    isFirst = false;
                    continue;
                }

                var value = selector(item);
                var compareResult = comparer.Compare(value, minValue);
                if (compareResult == 0)
                {
                    itemsWithMinValue.Add(item);
                }
                else if (compareResult < 0)
                {
                    itemWithMinValue = item;
                    minValue = value;
                    itemsWithMinValue.Clear();
                    itemsWithMinValue.Add(item);
                }
            }

            return itemsWithMinValue.ToArray();
        }

        public static T FirstWithMin<T, TSelection>(this IEnumerable<T> source, Func<T, TSelection> selector, IComparer comparer = null)
        {
            if (source == null || !source.Any())
                return default(T);

            if (comparer == null)
                comparer = Comparer<TSelection>.Default;

            var itemWithMinValue = source.First();
            TSelection minValue = selector(itemWithMinValue);

            foreach (var item in source)
            {
                var value = selector(item);
                if (comparer.Compare(value, minValue) < 0)
                {
                    itemWithMinValue = item;
                    minValue = value;
                }
            }

            return itemWithMinValue;
        }

        public static decimal WeightedSum<T>(this IEnumerable<T> source,
                                             Func<T, decimal> selector,
                                             Func<T, decimal> weightSelector)
        {
            var weightDict = source.ToDictionary(i => i, weightSelector);
            var sumWeight = weightDict.Values.Sum();
            if (sumWeight == 0)
                throw new InvalidOperationException("sum weight is zero");

            return source.Sum(i => selector(i) * weightDict[i] / sumWeight);
        }

        public static double WeightedSum<T>(this IEnumerable<T> source,
                                            Func<T, double> selector,
                                            Func<T, double> weightSelector)
        {
            var weightDict = source.ToDictionary(i => i, weightSelector);
            var sumWeight = weightDict.Values.Sum();
            if (sumWeight == 0)
                throw new InvalidOperationException("sum weight is zero");

            return source.Sum(i => selector(i) * weightDict[i] / sumWeight);
        }

        public static DiffResult<T> Diff<T>(this IEnumerable<T> source, IEnumerable<T> target, IEqualityComparer<T> equalityComparer)
        {
            var sourceItems = new HashSet<T>(source, equalityComparer);
            var targetItems = new HashSet<T>(target, equalityComparer);

            var added = targetItems.Where(t => !sourceItems.Contains(t)).ToArray();
            var removed = sourceItems.Where(t => !targetItems.Contains(t)).ToArray();
            var sourceShared = sourceItems.Where(t => targetItems.Contains(t)).ToArray();
            var shared = new SharedPair<T>[sourceShared.Length];
            for (var i = 0; i < sourceShared.Length; ++i)
            {
                var sourceSharedItem = sourceShared[i];
                var targetSharedItem = targetItems.First(t => equalityComparer.Equals(t, sourceSharedItem));
                shared[i] = new SharedPair<T>(sourceSharedItem, targetSharedItem);
            }

            return new DiffResult<T>(added, removed, shared);
        }

        public static DiffResult<T> Diff<T>(this IEnumerable<T> source, IEnumerable<T> target)
        {
            return Diff<T>(source, target, EqualityComparer<T>.Default);
        }

        public static bool Equals<T, TValue>(this IEnumerable<T> source, Func<T, TValue> selector)
        {
            if (!source.Any())
                return false;

            var first = source.First();
            var firstValue = selector(first);
            return source.All(s => selector(s).Equals(firstValue));
        }
    }
}

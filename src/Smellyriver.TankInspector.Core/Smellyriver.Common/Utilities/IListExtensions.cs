using System;
using System.Collections.Generic;

namespace Smellyriver.Utilities
{
    // ReSharper disable once InconsistentNaming
    internal static class IListExtensions
    {
        public static void AddRange<T>(this IList<T> @this, IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                @this.Add(value);
            }
        }
    }
}

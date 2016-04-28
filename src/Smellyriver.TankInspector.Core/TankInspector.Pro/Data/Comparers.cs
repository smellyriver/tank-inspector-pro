using System;
using System.Collections;
using System.Collections.Generic;
using Smellyriver.Collections;

namespace Smellyriver.TankInspector.Pro.Data
{
    public static class Comparers
    {
        public static IComparer<TSource> Create<TSource, TKey>(Func<TSource, TKey> projection)
        {
            return new ProjectionComparer<TSource, TKey>(projection);
        }

        private class AlwaysEqualComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                return 0;
            }
        }

        public static readonly IComparer AlwaysEqual = new AlwaysEqualComparer();
    }
}

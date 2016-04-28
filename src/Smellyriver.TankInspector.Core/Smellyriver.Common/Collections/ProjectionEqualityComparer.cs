using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smellyriver.Collections
{

    internal static class ProjectionEqualityComparer<TSource>
    {
        public static IEqualityComparer<TSource> Create<TKey>(Func<TSource, TKey> projection)
        {
            return new ProjectionEqualityComparer<TSource, TKey>(projection);
        }

        public static IEqualityComparer<TSource> Create<TKey>(Func<TSource, TKey> projection,
                                                              IEqualityComparer<TKey> comparer)
        {
            return new ProjectionEqualityComparer<TSource, TKey>(projection, comparer);
        }
    }

    internal class ProjectionEqualityComparer<TSource, TKey> : IEqualityComparer<TSource>
    {

        private readonly Func<TSource, TKey> _projection;
        private readonly IEqualityComparer<TKey> _comparer;

        public ProjectionEqualityComparer(Func<TSource, TKey> projection)
            : this(projection, null)
        {
        }

        public ProjectionEqualityComparer(Func<TSource, TKey> projection,
                                          IEqualityComparer<TKey> comparer)
        {
            if(projection == null)
                throw new ArgumentException("projection");

            this._comparer = comparer ?? EqualityComparer<TKey>.Default;
            this._projection = projection;
        }

        public bool Equals(TSource x, TSource y)
        {
            return _comparer.Equals(_projection(x), _projection(y));
        }

        public int GetHashCode(TSource obj)
        {
            return _comparer.GetHashCode(_projection(obj));
        }
    }
}

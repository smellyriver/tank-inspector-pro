using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smellyriver.Collections
{
    internal class ProjectionComparer<TSource, TKey> : IComparer<TSource>
    {
        private readonly Func<TSource, TKey> _projection;
        private readonly IComparer<TKey> _comparer;

        public ProjectionComparer(Func<TSource, TKey> projection)
            : this(projection, null)
        {
        }

        public ProjectionComparer(Func<TSource, TKey> projection,
                                  IComparer<TKey> comparer)
        {
            if(projection == null)
                throw new ArgumentException("projection");

            this._comparer = comparer ?? Comparer<TKey>.Default;
            this._projection = projection;
        }

        public int Compare(TSource x, TSource y)
        {
            // Don't want to project from nullity
            if (x == null && y == null)
            {
                return 0;
            }
            if (x == null)
            {
                return -1;
            }
            if (y == null)
            {
                return 1;
            }
            return _comparer.Compare(_projection(x), _projection(y));
        }
    }
}

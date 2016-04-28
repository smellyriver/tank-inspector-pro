using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Smellyriver.Utilities;

namespace Smellyriver.TankInspector.Pro.Data
{
    public class CachedXQueryable : IXQueryable
    {
        private readonly IXQueryable _wrapped;
        private readonly Dictionary<string, object> _cache;

        public CachedXQueryable(IXQueryable wrapped)
        {
            if (wrapped == null)
                throw new ArgumentNullException("wrapped");

            _wrapped = wrapped;

            _cache = new Dictionary<string, object>();
        }

        private T GetCache<T>(string xpath, Func<T> factory)
        {
            return (T)_cache.GetOrCreate(xpath, () => (object)factory());
        }

        public virtual string Xml
        {
            get { return _wrapped.Xml; }
        }

        public virtual string Name
        {
            get { return _wrapped.Name; }
        }

        public virtual string Value
        {
            get { return _wrapped.Value; }
        }

        public virtual string Text
        {
            get { return _wrapped.Text; }
        }

        public virtual IXQueryable Query(string xpath)
        {
            return this.GetCache(xpath, () => _wrapped.Query(xpath));
        }

        public virtual string QueryValue(string xpath)
        {
            return this.GetCache(xpath, () => _wrapped.QueryValue(xpath));
        }

        public virtual IEnumerable<IXQueryable> QueryMany(string xpath)
        {
            return this.GetCache(xpath, () => _wrapped.QueryMany(xpath));
        }

        public virtual IEnumerable<string> QueryManyValues(string xpath)
        {
            return this.GetCache(xpath, () => _wrapped.QueryManyValues(xpath));
        }

        public virtual XElement ToElement()
        {
            return _wrapped.ToElement();
        }

        public virtual string this[string xpath]
        {
            get { return this.QueryValue(xpath); }
        }

        public virtual string this[string xpathFormat, params string[] args]
        {
            // ReSharper disable once CoVariantArrayConversion
            get { return this.QueryValue(string.Format(xpathFormat, args)); }
        }

        public virtual IEnumerator<IXQueryable> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}

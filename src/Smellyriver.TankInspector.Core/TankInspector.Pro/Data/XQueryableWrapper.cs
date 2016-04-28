using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Smellyriver.TankInspector.Pro.Data
{
    public class XQueryableWrapper : IXQueryable
    {
        private readonly IXQueryable _wrapped;

        public XQueryableWrapper(IXQueryable wrapped)
        {
            if (wrapped == null)
                throw new ArgumentNullException("wrapped");

            _wrapped = wrapped;
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
            return _wrapped.Query(xpath);
        }

        public virtual string QueryValue(string xpath)
        {
            return _wrapped.QueryValue(xpath);
        }

        public virtual IEnumerable<IXQueryable> QueryMany(string xpath)
        {
            return _wrapped.QueryMany(xpath);
        }

        public virtual IEnumerable<string> QueryManyValues(string xpath)
        {
            return _wrapped.QueryManyValues(xpath);
        }

        public virtual XElement ToElement()
        {
            return _wrapped.ToElement();
        }

        public virtual string this[string xpath]
        {
            get { return _wrapped[xpath]; }
        }

        public virtual string this[string xpathFormat, params string[] args]
        {
            get { return _wrapped[xpathFormat, args]; }
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

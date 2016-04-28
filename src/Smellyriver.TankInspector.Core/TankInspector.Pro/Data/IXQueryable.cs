using System.Collections.Generic;
using System.Xml.Linq;

namespace Smellyriver.TankInspector.Pro.Data
{
    public interface IXQueryable : IEnumerable<IXQueryable>
    {
        /// <summary>
        /// Get the XML representing this IXQueryable object
        /// </summary>
        string Xml { get; }

        /// <summary>
        /// Get the element name of this IXQueryable object
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get the element value of this IXQueryable object
        /// </summary>
        string Value { get; }

        /// <summary>
        /// Get the text value of this IXQueryable object if the first node is a text node
        /// </summary>
        string Text { get; }
        IXQueryable Query(string xpath);
        string QueryValue(string xpath);
        IEnumerable<IXQueryable> QueryMany(string xpath);
        IEnumerable<string> QueryManyValues(string xpath);
        XElement ToElement();
        string this[string xpath] { get; }
        string this[string xpathFormat, params string[] args] { get; }
    }
}

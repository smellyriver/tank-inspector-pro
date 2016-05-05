using System;
using System.Xml.Linq;

namespace Smellyriver.TankInspector.Pro.Data
{
    public static class XElementExtensions
    {
        public static IXQueryable ToXQueryable(this XElement element)
        {
            if (element == null)
                return null;

            return new XQueryable(element);
        }

        public static XElement ExistedElement(this XElement element, XName name)
        {
            var e = element.Element(name);
            if (e == null)
                throw new InvalidOperationException("element not found");

            return e;
        }

        public static XAttribute ExistedAttribute(this XElement element, XName name)
        {
            var a = element.Attribute(name);
            if (a == null)
                throw new InvalidOperationException("attribute not found");

            return a;
        }
    }
}

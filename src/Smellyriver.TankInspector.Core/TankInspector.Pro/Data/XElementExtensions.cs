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
    }
}

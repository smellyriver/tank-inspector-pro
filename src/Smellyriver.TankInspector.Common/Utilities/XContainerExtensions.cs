using System.Xml.Linq;

namespace Smellyriver.TankInspector.Common.Utilities
{
    public static class XContainerExtensions
    {
        public static void TrimText(this XContainer @this)
        {
            foreach (var decendant in @this.DescendantNodes())
            {
                var decendantText = decendant as XText;
                if (decendantText != null)
                    decendantText.Value = decendantText.Value.Trim();


            }
        }

        public static void AddOrReplace(this XContainer @this, XElement element)
        {
            var oldElement = @this.Element(element.Name);
            if (oldElement != null)
                oldElement.ReplaceWith(element);
            else
                @this.Add(element);
        }
    }
}

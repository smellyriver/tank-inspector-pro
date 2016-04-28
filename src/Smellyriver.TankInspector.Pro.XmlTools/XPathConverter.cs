using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Smellyriver.TankInspector.Pro.XmlTools
{
    class XPathConverter : IValueConverter
    {
        private readonly string _xpath;

        public XPathConverter(string xpath)
        {
            _xpath = xpath;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var element = value as XElement;

            if (element == null)
                return null;

            object result;
            try
            {
                result = element.XPathEvaluate(_xpath);
            }
            catch (XPathException ex)
            {
                return this.L("tech_tree", "xpath_error", ex.Message);
            }

            if (result == null)
                return null;

            var enumerableResult = result as IEnumerable;

            if (enumerableResult == null)
            {
                return result.ToString();
            }
            else
            {
                var firstResult = enumerableResult.Cast<object>().FirstOrDefault();
                if (firstResult != null)
                {
                    var textResult = firstResult as XText;
                    if (textResult != null)
                        return textResult.Value;

                    var elementResult = firstResult as XElement;
                    if (elementResult != null)
                        return elementResult.Value;

                    var attributeResult = firstResult as XAttribute;
                    if (attributeResult != null)
                        return attributeResult.Value;
                }

            }



            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

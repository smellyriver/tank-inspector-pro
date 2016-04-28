using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Smellyriver.TankInspector.Pro.Data
{
    public class XQueryable : IXQueryable
    {
        private readonly XElement _element;

        public XElement Element { get { return _element; } }

        public string Xml
        {
            get
            {
                this.UpdateElement();
                return _element.ToString();
            }
        }

        public string Name
        {
            get { return _element.Name.ToString(); }
        }

        public string Value
        {
            get
            {
                this.UpdateElement();
                return _element.Value;
            }
        }

        public string Text
        {
            get
            {
                this.UpdateElement();
                if (this.Element.FirstNode is XText)
                    return ((XText)this.Element.FirstNode).Value;

                return null;
            }
        }

        public XQueryable(XElement element)
        {
            _element = element;
        }

        protected virtual void UpdateElement()
        {

        }

        public IXQueryable Query(string xpath)
        {
            this.UpdateElement();

            return _element.XPathSelectElement(xpath).ToXQueryable();
        }

        public string QueryValue(string xpath)
        {
            this.UpdateElement();

            return this.GetXPathEvaluateResultValue(_element.XPathEvaluate(xpath));
        }

        private string GetXPathEvaluateResultValue(object result)
        {
            var enumerableResult = result as IEnumerable;
            if (enumerableResult != null)
            {
                foreach (var resultItem in enumerableResult)
                {
                    var attribute = resultItem as XAttribute;
                    if (attribute != null)
                        return attribute.Value;

                    var element = resultItem as XElement;
                    if (element != null)
                        return element.Value;

                    var navigator = resultItem as XPathNavigator;
                    if (navigator != null)
                        return navigator.Value;
                }

                return null;
            }

            var attributeNode = result as XAttribute;
            if (attributeNode != null)
                return attributeNode.Value;

            var elementNode = result as XElement;
            if (elementNode != null)
                return elementNode.Value;

            if (result is double)
            {
                return ((double)result).ToString(CultureInfo.InvariantCulture);
            }

            return result.ToString();
        }

        public IEnumerable<string> QueryManyValues(string xpath)
        {
            this.UpdateElement();

            var result = _element.XPathEvaluate(xpath);

            var values = new List<string>();

            var enumerableResult = result as IEnumerable;
            if (enumerableResult != null)
            {
                foreach (var item in enumerableResult)
                    values.Add(this.GetXPathEvaluateResultValue(item));

                return values;
            }

            values.Add(result.ToString());
            return values;
        }

        public IEnumerable<IXQueryable> QueryMany(string xpath)
        {
            this.UpdateElement();

            return _element.XPathSelectElements(xpath).Select(e => e.ToXQueryable());
        }

        public XElement ToElement()
        {
            this.UpdateElement();
            return new XElement(_element);
        }

        public string this[string xpath]
        {
            get { return this.QueryValue(xpath); }
        }

        public string this[string xpathFormat, params string[] args]
        {
            get { return this.QueryValue(xpathFormat, args); }
        }

        public IEnumerator<IXQueryable> GetEnumerator()
        {
            return _element.Elements().Select(e => e.ToXQueryable()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}

using System;
using System.Linq;
using System.Xml.Linq;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.Pro.Data;

namespace Smellyriver.TankInspector.Pro.Repository.XmlProcessing
{

    internal static class XElementExtensions
    {
        public static XElement ForEachElement(this XElement element, Action<XElement> action)
        {
            foreach (var childElement in element.Elements())
            {
                action(childElement);
            }

            return element;
        }

        public static XElement ForEachElementStable(this XElement element, Action<XElement> action)
        {
            foreach (var childElement in element.Elements().ToArray())
            {
                action(childElement);
            }

            return element;
        }

        public static XElement Select(this XElement element, string name, Action<XElement> action)
        {
            var childElement = name == null ? element : element.Element(name);
            if (childElement != null)
                action(childElement);

            return element;
        }

        public static XElement RemoveElement(this XElement element, string name)
        {
            element.ExistedElement(name).Remove();
            return element;
        }


        public static XElement NameToAttribute(this XElement element, string newName, string attributeName = "key")
        {
            var key = element.Name;
            element.Name = newName;
            element.SetAttributeValue(attributeName, key);
            return element;
        }

        public static XElement TextToAttribute(this XElement element, string attributeName)
        {
            var textNode = element.FirstNode as XText;
            if (textNode != null)
            {
                var text = textNode.Value.Trim();
                textNode.Remove();
                element.SetAttributeValue(attributeName, text);
            }

            return element;
        }


        public static XElement TextToAttributes(this XElement element, params string[] attributeNames)
        {
            var textNode = element.FirstNode as XText;
            if (textNode != null)
            {
                var text = textNode.Value.Trim();
                textNode.Remove();
                var sections = text.Split(' ');
                for (var i = 0; i < sections.Length; ++i)
                {
                    element.SetAttributeValue(attributeNames[i], sections[i]);
                }

            }

            return element;
        }


        public static XElement TextToElement(this XElement element, string elementName)
        {
            var textNode = element.FirstNode as XText;
            if (textNode != null)
            {
                var text = textNode.Value.Trim();
                textNode.Remove();
                element.Add(new XElement(elementName, text));
            }

            return element;
        }

        public static XElement TextToElements(this XElement element, params string[] elementNames)
        {
            var textNode = element.FirstNode as XText;
            if (textNode != null)
            {
                var text = textNode.Value.Trim();
                textNode.Remove();
                var sections = text.Split(' ');
                for (var i = 0; i < sections.Length; ++i)
                {
                    element.Add(new XElement(elementNames[i], sections[i]));
                }

            }

            return element;
        }


        public static XElement TextToElementList(this XElement element, string elementName)
        {
            var textNode = element.FirstNode as XText;
            if (textNode != null)
            {
                var text = textNode.Value.Trim();
                textNode.Remove();
                var sections = text.Split(CharEx.Whitespaces, StringSplitOptions.RemoveEmptyEntries);
                foreach (string section in sections)
                {
                    element.Add(new XElement(elementName, section.Trim()));
                }
            }

            return element;
        }

        public static XElement ApplyProcessing(this XElement element, Action<XElement> processing)
        {
            processing?.Invoke(element);
            return element;
        }

        public static XElement TrimNameTail(this XElement element)
        {
            var name = element.Name.ToString();

            // remove '.xml'
            element.Name = name.Substring(0, name.Length - 4);
            return element;
        }

        public static XElement Rename(this XElement element, string newName)
        {
            element.Name = newName;
            return element;
        }

        public static XElement RenameElement(this XElement element, string elementName, string newName)
        {
            return element.Select(elementName, e => e.Name = newName);
        }

        public static XElement ElementToAttribute(this XElement element,
                                                  string elementName,
                                                  string attributeName = null)
        {
            attributeName = attributeName ?? elementName;
            var childElement = element.Element(elementName);
            if (childElement != null)
            {
                element.SetAttributeValue(attributeName, childElement.Value);
                childElement.Remove();
            }

            return element;
        }

        public static XElement ElementToBooleanAttribute(this XElement element,
                                                          string elementName,
                                                          string attributeName,
                                                          string trueValue = "true",
                                                          string falseValue = "false")
        {
            var childElement = element.Element(elementName);
            if (childElement != null)
            {
                element.SetAttributeValue(attributeName, trueValue);
                childElement.Remove();
            }
            else
                element.SetAttributeValue(attributeName, falseValue);

            return element;
        }


        public static XElement TextToBooleanAttribute(this XElement element,
                                                       string matchText,
                                                       string attributeName,
                                                       string trueValue = "true",
                                                       string falseValue = "false")
        {
            var textNode = element.FirstNode as XText;
            if (textNode != null && textNode.Value == matchText)
            {
                element.SetAttributeValue(attributeName, trueValue);
                textNode.Remove();
            }
            else
                element.SetAttributeValue(attributeName, falseValue);

            return element;
        }


        public static XElement ProcessElements(this XElement element, Action<XElement> action)
        {
            return element.ProcessElements(null, action);
        }

        public static XElement ProcessElements(this XElement element, string name, Action<XElement> action)
        {
            return element.Select(name, e => e.ForEachElement(action));
        }

        public static XElement ProcessElementsStable(this XElement element, Action<XElement> action)
        {
            return element.ProcessElementsStable(null, action);
        }

        public static XElement ProcessElementsStable(this XElement element, string name, Action<XElement> action)
        {
            return element.Select(name, e => e.ForEachElementStable(action));
        }

        public static XElement ProcessPriceElement(this XElement element, string priceElementName = "price")
        {
            return element.Select(priceElementName, e => e.ElementToBooleanAttribute("gold", "currency", "gold", "credit"));
        }

        public static XElement AppendCommonArmorGroup(this XElement element,
                                                      XElement commonVehicleData,
                                                      string armorGroupName)
        {
            var material = commonVehicleData.ExistedElement("materials")
                                            .Elements()
                                            .FirstOrDefault(m => m.Attribute("key").Value == armorGroupName);

            if (material == null)
                return element;

            var armorNode = new XElement(material);
            armorNode.Name = "armor";
            armorNode.SetAttributeValue("key", armorGroupName);

            element.Add(armorNode);

            return element;
        }

        public static XElement ProcessArmorList(this XElement element,
                                                XElement commonVehicleData,
                                                string armorElementName = "armor",
                                                string primaryArmorElementName = "primaryArmor")
        {
            var armorElement = element.ExistedElement(armorElementName);
            return element.Select(armorElementName,
                                  a => a.ProcessElementsStable(e =>
                                                               {
                                                                   var material = commonVehicleData.ExistedElement("materials")
                                                                                                   .Elements()
                                                                                                   .First(m => m.Attribute("key").Value == e.Name);
                                                                   var armorNode = new XElement(material)
                                                                   {
                                                                       Name = "armor"
                                                                   };
                                                                   foreach (var el in e.TextToElement("thickness").Elements())
                                                                       armorNode.AddOrReplace(new XElement(el));

                                                                   e.ReplaceWith(armorNode);
                                                               }))
                          .Select(primaryArmorElementName, a => a.TextToElements("front", "side", "rear"))
                          .ProcessElements(primaryArmorElementName,
                                           a =>
                                           {
                                               var key = a.Value;
                                               a.TextToAttribute("key");
                                               var armorElementReference = armorElement.Elements().FirstOrDefault(e => e.Attribute("key").Value == key);
                                               if (armorElementReference != null)
                                                   a.Value = armorElementReference.ExistedElement("thickness").Value;
                                           });
        }

        public static XElement ProcessShellList(this XElement element, string name = "shots")
        {
            return element.Select(name, s => s.ProcessElements(e => e.NameToAttribute("shell")
                                                                     .Select("piercingPower", p => p.TextToElements("p100", "p400"))));
        }


        public static XElement ProcessTankModuleListNode(this XElement element, string name, string elementName, LocalGameClientLocalization localization, Action<XElement> additionalProcessing = null)
        {
            return element.Select(name, list =>
            {
                list.ProcessElements(
                    e => e.NameToAttribute(elementName)
                          .LocalizeValue("userString", localization)
                          .TextToBooleanAttribute("shared", "shared")
                          .ProcessElements("unlocks", u => u.TextToAttribute("key"))
                          .ApplyProcessing(additionalProcessing));
            });
        }

        public static XElement LocalizeValue(this XElement element, string name, LocalGameClientLocalization localization, bool writeKey = false)
        {
            return element.Select(name,
                                  e =>
                                  {
                                      if (writeKey)
                                          e.SetAttributeValue("key", e.Value);

                                      e.Value = localization.GetLocalizedString(e.Value);
                                  });
        }

        public static XElement LocalizeValue(this XElement element, LocalGameClientLocalization localization)
        {
            return element.LocalizeValue(null, localization);
        }
    }

}

using System;
using System.Xml.Linq;

namespace Smellyriver.TankInspector.Pro.Data.Tank
{
    class ElementChangedEventArgs : EventArgs
    {
        public XElement Element { get; private set; }
        public ElementChangedEventArgs(XElement element)
        {
            this.Element = element;
        }
    }
}

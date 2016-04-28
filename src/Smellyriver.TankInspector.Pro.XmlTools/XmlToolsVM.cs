using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Pro.Modularity.Features;

namespace Smellyriver.TankInspector.Pro.XmlTools
{
    class XmlToolsVM : NotificationObject
    {
        public IXmlViewer XmlViewer { get; set; }

        public XPathVM XPathTools { get; private set; }
        public XsltVM XsltTools { get; private set; }

        public XmlToolsModule Module { get; private set; }

        public XmlToolsVM(XmlToolsModule module)
        {
            this.Module = module;
            this.XPathTools = new XPathVM(this);
            this.XsltTools = new XsltVM(this);
        }
    }
}

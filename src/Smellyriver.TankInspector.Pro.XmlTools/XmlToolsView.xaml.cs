using System.Windows.Controls;

namespace Smellyriver.TankInspector.Pro.XmlTools
{
    public partial class XmlToolsView : UserControl
    {
        internal XmlToolsVM ViewModel
        {
            get { return this.DataContext as XmlToolsVM; }
            set { this.DataContext = value; }
        }

        public XmlToolsView()
        {
            this.InitializeComponent();
        }
    }
}

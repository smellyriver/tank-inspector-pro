using System.Windows.Controls;

namespace Smellyriver.TankInspector.Pro.TechTree
{
    public partial class TechTreeDocumentView : UserControl
    {
        internal TechTreeDocumentVM ViewModel
        {
            get { return this.DataContext as TechTreeDocumentVM; }
            set { this.DataContext = value; }
        }

        public TechTreeDocumentView()
        {
            InitializeComponent();
        }
    }
}

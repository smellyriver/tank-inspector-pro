using System.Windows.Controls;

namespace Smellyriver.TankInspector.Pro.StatsInspector
{
    public partial class StatsDocumentView : UserControl
    {

        internal StatsDocumentVM ViewModel
        {
            get { return this.DataContext as StatsDocumentVM; }
            set { this.DataContext = value; }
        }

        public StatsDocumentView()
        {
            InitializeComponent();
        }
    }
}

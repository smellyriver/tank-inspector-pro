using System.Windows.Controls;

namespace Smellyriver.TankInspector.Pro.StatChangesView
{
    public partial class StatChangesView : UserControl
    {
        internal StatChangesVM ViewModel
        {
            get { return this.DataContext as StatChangesVM; }
            set { this.DataContext = value; }
        }

        public StatChangesView()
        {
            InitializeComponent();
        }
    }
}

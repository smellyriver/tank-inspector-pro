using System.Windows.Controls;

namespace Smellyriver.TankInspector.Pro.TankConfigurator
{
    public partial class TankConfigView : UserControl
    {
        internal TankConfigVM ViewModel
        {
            get { return this.DataContext as TankConfigVM; }
            set { this.DataContext = value; }
        }

        public TankConfigView()
        {
            InitializeComponent();
        }
    }
}

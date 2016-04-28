using System.Windows.Controls;

namespace Smellyriver.TankInspector.Pro.CustomizationConfigurator
{
    public partial class CustomizationConfigView : UserControl
    {
        internal CustomizationConfigVM ViewModel
        {
            get { return this.DataContext as CustomizationConfigVM; }
            set { this.DataContext = value; }
        }

        public CustomizationConfigView()
        {
            InitializeComponent();
        }
    }
}

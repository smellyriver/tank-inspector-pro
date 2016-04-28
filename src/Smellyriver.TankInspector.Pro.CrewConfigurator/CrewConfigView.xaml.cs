using System.Windows.Controls;

namespace Smellyriver.TankInspector.Pro.CrewConfigurator
{
    public partial class CrewConfigView : UserControl
    {

        internal CrewConfigVM ViewModel
        {
            get { return this.DataContext as CrewConfigVM; }
            set { this.DataContext = value; }
        }

        public CrewConfigView()
        {
            InitializeComponent();
        }
    }
}

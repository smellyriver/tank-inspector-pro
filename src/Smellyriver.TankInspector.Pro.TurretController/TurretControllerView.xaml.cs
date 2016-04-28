using System.Windows.Controls;

namespace Smellyriver.TankInspector.Pro.TurretController
{
    public partial class TurretControllerView : UserControl
    {
        internal TurretControllerVM ViewModel
        {
            get{return this.DataContext as TurretControllerVM; }
            set{this.DataContext = value; }
        }

        public TurretControllerView()
        {
            InitializeComponent();
        }
    }
}

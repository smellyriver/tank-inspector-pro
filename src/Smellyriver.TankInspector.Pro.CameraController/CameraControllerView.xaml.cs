using System.Windows.Controls;

namespace Smellyriver.TankInspector.Pro.CameraController
{
    public partial class CameraControllerView : UserControl
    {
        internal CameraControllerVM ViewModel
        {
            get { return this.DataContext as CameraControllerVM; }
            set { this.DataContext = value; }
        }

        public CameraControllerView()
        {
            InitializeComponent();
        }
    }
}

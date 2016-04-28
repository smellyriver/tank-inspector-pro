using System.Windows.Controls;

namespace Smellyriver.TankInspector.Pro.InteractiveConsole
{
    public partial class InteractiveView : UserControl
    {
        internal InteractiveVM ViewModel
        {
            get { return this.DataContext as InteractiveVM; }
            private set { this.DataContext = value; }
        }

        public InteractiveView()
        {
            this.ViewModel = new InteractiveVM();
            InitializeComponent();
        }
    }
}

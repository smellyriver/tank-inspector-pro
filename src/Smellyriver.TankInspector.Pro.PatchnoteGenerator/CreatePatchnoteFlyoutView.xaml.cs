using System.Windows.Controls;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator
{
    /// <summary>
    /// Interaction logic for CreatePatchnoteFlyoutView.xaml
    /// </summary>
    public partial class CreatePatchnoteFlyoutView : UserControl
    {
        internal CreatePatchnoteFlyoutVM ViewModel
        {
            get { return this.DataContext as CreatePatchnoteFlyoutVM; }
            set { this.DataContext = value; }
        }

        public CreatePatchnoteFlyoutView()
        {
            InitializeComponent();
        }
    }
}

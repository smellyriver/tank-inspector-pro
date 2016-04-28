using System.Windows.Controls;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    /// <summary>
    /// Interaction logic for ComparisonSelectorFlyoutView.xaml
    /// </summary>
    public partial class ComparisonSelectorFlyoutView : UserControl
    {
        internal ComparisonSelectorFlyoutVM ViewModel
        {
            get { return this.DataContext as ComparisonSelectorFlyoutVM; }
            set { this.DataContext = value; }
        }

        public ComparisonSelectorFlyoutView()
        {
            InitializeComponent();
        }
    }
}

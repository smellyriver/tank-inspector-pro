using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace Smellyriver.TankInspector.Pro.TankMuseum
{
    public partial class TankMuseumView : UserControl
    {
        internal TankMuseumVM ViewModel
        {
            get { return this.DataContext as TankMuseumVM; }
            set { this.DataContext = value; }
        }

        public TankMuseumView()
        {
            InitializeComponent();
            this.ViewModel = new TankMuseumVM();
        }

        private void TankList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var dataGrid = (DataGrid)sender;
            this.ViewModel.SelectedTanks = dataGrid.SelectedItems.OfType<TankVM>().ToArray();
        }

    }
}

using System.Windows.Controls;

namespace Smellyriver.TankInspector.Pro.InternalModules.RepositoryManager
{

    public partial class RepositoryManagerDocumentView : UserControl
    {

        internal RepositoryManagerDocumentVM ViewModel
        {
            get { return this.DataContext as RepositoryManagerDocumentVM; }
            set { this.DataContext = value; }
        }

        public RepositoryManagerDocumentView()
        {
            InitializeComponent();
        }
    }
}

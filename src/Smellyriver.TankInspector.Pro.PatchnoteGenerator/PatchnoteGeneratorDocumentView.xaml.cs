using System.Windows.Controls;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator
{
    public partial class PatchnoteGeneratorDocumentView : UserControl
    {
        internal PatchnoteGeneratorDocumenVM ViewModel
        {
            get { return this.DataContext as PatchnoteGeneratorDocumenVM; }
            set { this.DataContext = value; }
        }

        public PatchnoteGeneratorDocumentView()
        {
            InitializeComponent();
        }
    }
}

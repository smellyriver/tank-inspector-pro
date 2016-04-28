using Smellyriver.TankInspector.Pro.Modularity;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    class AvailableComparisonVM : AvailableComparisonVMBase
    {
        public string Name { get { return this.DocumentVM.Title; } }

        public DocumentInfo Document { get; private set; }
        public StatComparisonDocumentVM DocumentVM { get; }

        public AvailableComparisonVM(DocumentInfo document)
        {
            this.Document = document;
            this.DocumentVM = ((StatComparisonDocumentView)document.Content).ViewModel;
        }
    }
}

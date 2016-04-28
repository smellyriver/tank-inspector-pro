using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Smellyriver.TankInspector.Pro.Modularity
{
    public interface IDocumentManager
    {
        bool IsRestoringDocument { get; }
        event EventHandler DocumentsRestored;
        Task<DocumentInfo> OpenDocument(Uri uri);
        ReadOnlyObservableCollection<DocumentInfo> Documents { get; }
        DocumentInfo ActiveDocument { get; }
        void SelectDocument(DocumentInfo document);

        event EventHandler<DocumentEventArgs> DocumentSelected;
        event EventHandler<DocumentEventArgs> ActiveDocumentChanged;
    }
}

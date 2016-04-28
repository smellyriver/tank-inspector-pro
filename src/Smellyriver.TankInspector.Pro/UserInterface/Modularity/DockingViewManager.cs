using System;
using System.Collections.ObjectModel;
using Smellyriver.TankInspector.Pro.Modularity;

namespace Smellyriver.TankInspector.Pro.UserInterface.Modularity
{
    public partial class DockingViewManager
    {
        public static DockingViewManager Instance { get; private set; }

        static DockingViewManager()
        {
            DockingViewManager.Instance = new DockingViewManager();
        }

        private readonly PanelManagerImpl _panelManager;
        private readonly DocumentManagerImpl _documentManager;

        public IPanelManager PanelManager { get { return _panelManager; } }
        public IDocumentManager DocumentManager { get { return _documentManager; } }

        internal ReadOnlyObservableCollection<PanelInfo> Panels { get { return _panelManager.Panels; } }
        internal ReadOnlyObservableCollection<DocumentInfo> Documents { get { return _documentManager.Documents; } }
        internal event EventHandler<DocumentEventArgs> DocumentSelected;
        internal DocumentInfo ActiveDocument
        {
            get { return _documentManager.ActiveDocument; }
            set { _documentManager.ActiveDocument = value; }
        }

        private DockingViewManager()
        {
            _panelManager = new PanelManagerImpl();
            _documentManager = new DocumentManagerImpl();

            _documentManager.ActiveDocumentChanged += DocumentManager_ActiveDocumentChanged;
            _documentManager.DocumentSelected += DocumentManager_DocumentSelected;
        }

        void DocumentManager_DocumentSelected(object sender, DocumentEventArgs e)
        {
            if (this.DocumentSelected != null)
                this.DocumentSelected(this, e);
        }

        void DocumentManager_ActiveDocumentChanged(object sender, DocumentEventArgs e)
        {
            foreach (var panel in _panelManager.Panels)
            {
                if (panel.ActiveDocumentChangedCallback != null)
                    panel.ActiveDocumentChangedCallback(e.Document);
            }
        }


        internal void CloseTemporaryDocuments()
        {
            _documentManager.CloseTemporaryDocuments();
        }

        internal void CloseDocument(DocumentInfo document)
        {
            _documentManager.CloseDocument(document);
        }

        internal void SaveDocumentRestorationInfo()
        {
            _documentManager.SaveDocumentRestorationInfo();
        }

    }
}

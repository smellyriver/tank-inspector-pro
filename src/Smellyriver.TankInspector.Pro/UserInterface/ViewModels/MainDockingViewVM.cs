using System;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Smellyriver.Collections;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Common.Collections;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;

namespace Smellyriver.TankInspector.Pro.UserInterface.ViewModels
{

    class MainDockingViewVM : NotificationObject
    {
        public ViewModelMap<PanelInfo, PanelVM> Panels { get; private set; }
        public ViewModelMap<DocumentInfo, DocumentVM> Documents { get; }

        private DocumentVM _activeDocument;
        public DocumentVM ActiveDocument
        {
            get { return _activeDocument; }
            set
            {
                _activeDocument = value;
                DockingViewManager.Instance.ActiveDocument = _activeDocument == null ? null : _activeDocument.Document;
            }
        }

        public ShellVM Shell { get; private set; }
        private readonly CommandBindingCollection _commandBindings;

        public bool IsRestoringDocuments
        {
            get { return DockingViewManager.Instance.DocumentManager.IsRestoringDocument; }
        }

        public event EventHandler DocumentsRestored;

        public MainDockingViewVM(ShellVM shell, CommandBindingCollection commandBindings)
        {
            this.Shell = shell;
            _commandBindings = commandBindings;

            this.Panels = new ViewModelMap<PanelInfo, PanelVM>(DockingViewManager.Instance.Panels, p => new PanelVM(p));
            this.Documents = new ViewModelMap<DocumentInfo, DocumentVM>(DockingViewManager.Instance.Documents,
                d =>
                {
                    var document = new DocumentVM(d);
                    App.CompositionContainer.SatisfyImportsOnce(document);
                    return document;
                });

            this.Documents.CollectionChanged += Documents_CollectionChanged;

            DockingViewManager.Instance.DocumentSelected += DockingViewManager_DocumentSelected;
            DockingViewManager.Instance.DocumentManager.DocumentsRestored += DocumentManager_DocumentsRestored;

            _commandBindings.Add(new CommandBinding(ApplicationCommands.Close, this.ExecuteCloseCommmand, this.CanExecuteCloseCommand));
        }

        void DocumentManager_DocumentsRestored(object sender, EventArgs e)
        {
            if (this.DocumentsRestored != null)
                this.DocumentsRestored(this, e);
        }

        void Documents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
        }

        private void CanExecuteCloseCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DockingViewManager.Instance.ActiveDocument != null;
        }

        private void ExecuteCloseCommmand(object sender, ExecutedRoutedEventArgs e)
        {
            if (DockingViewManager.Instance.ActiveDocument != null)
                DockingViewManager.Instance.CloseDocument(DockingViewManager.Instance.ActiveDocument);
        }

        void DockingViewManager_DocumentSelected(object sender, DocumentEventArgs e)
        {
            var documentVM = this.Documents.Where(d => d.Document == e.Document).FirstOrDefault();
            if (documentVM != null)
                documentVM.IsSelected = true;
        }

        public void CloseTemporaryDocuments()
        {
            DockingViewManager.Instance.CloseTemporaryDocuments();
        }

        public void CloseDocument(DocumentVM document)
        {
            DockingViewManager.Instance.CloseDocument(document.Document);
        }

        public void SaveDocumentRestorationInfo()
        {
            DockingViewManager.Instance.SaveDocumentRestorationInfo();
        }


    }
}

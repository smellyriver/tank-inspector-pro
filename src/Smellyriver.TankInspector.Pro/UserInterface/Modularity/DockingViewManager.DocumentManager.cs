using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Smellyriver.TankInspector.Common.Serialization;
using Smellyriver.TankInspector.Pro.IO;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Tasks;
using Smellyriver.TankInspector.Common.Utilities;

namespace Smellyriver.TankInspector.Pro.UserInterface.Modularity
{
    partial class DockingViewManager
    {
        internal partial class DocumentManagerImpl : IDocumentManager
        {
            

            private static readonly string s_documentRestorationConfigFile = ApplicationPath.GetConfigPath("documents.config");

            private readonly ObservableCollection<DocumentInfo> _documents;
            public ReadOnlyObservableCollection<DocumentInfo> Documents { get; }

            public event EventHandler<DocumentEventArgs> DocumentSelected;
            public event EventHandler<DocumentEventArgs> ActiveDocumentChanged;

            private DocumentInfo _activeDocument;
            public DocumentInfo ActiveDocument
            {
                get { return _activeDocument; }
                internal set
                {
                    if (_activeDocument != value)
                    {
                        _activeDocument = value;
                        this.OnActiveDocumentChanged();
                    }
                }
            }

            public bool IsRestoringDocument { get; private set; }
            public event EventHandler DocumentsRestored;

            public DocumentManagerImpl()
            {
                _documents = new ObservableCollection<DocumentInfo>();
                this.Documents = new ReadOnlyObservableCollection<DocumentInfo>(_documents);

                this.OnActiveDocumentChanged();
            }

            private void OnDocumentsRestored()
            {
                if (this.DocumentsRestored != null)
                    this.DocumentsRestored(this, EventArgs.Empty);
            }


            public void OpenDocument(DocumentInfo document)
            {
                _documents.Add(document);
                this.OnDocumentSelected(document);
            }

            public void CloseDocument(DocumentInfo document)
            {
                _documents.Remove(document);
            }

            public void CloseTemporaryDocuments()
            {
                _documents.RemoveWhere(d => d.IsTemporary);
            }

            private void OnDocumentSelected(DocumentInfo document)
            {
                if (DocumentSelected != null)
                    this.DocumentSelected(this, new DocumentEventArgs(document));
            }


            public Task<DocumentInfo> OpenDocument(Uri uri)
            {
                return this.OpenDocument(uri, Guid.NewGuid(), null);
            }

            private Task<DocumentInfo> OpenDocument(Uri uri, Guid guid, string persistentInfo)
            {
                var task = new OpenDocumentTask(this, uri, guid, persistentInfo);
                return Task.Factory.StartNew(
                    () =>
                    {
                        var taskManager = (TaskManager)Application.Current.Dispatcher.Invoke(new Func<TaskManager>(() => TaskManager.RunTask(task)));
                        taskManager.CurrentOperation.Wait();
                        return task.DocumentInfo;
                    });
            }

            public void SaveDocumentRestorationInfo()
            {
                var restorationInfo = _documents.Where(d => !d.IsTemporary)
                                                .Select(d => new DocumentRestorationInfo(d))
                                                .ToArray();

                Serializer.DataContractSerialize(restorationInfo, s_documentRestorationConfigFile);
            }

            public void SelectDocument(DocumentInfo document)
            {
                this.OnDocumentSelected(document);
            }

            private void OnActiveDocumentChanged()
            {
                if (this.ActiveDocumentChanged != null)
                    this.ActiveDocumentChanged(this, new DocumentEventArgs(this.ActiveDocument));

            }

        }
    }
}

using System;
using System.Linq;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Tasks;

namespace Smellyriver.TankInspector.Pro.UserInterface.Modularity
{
    partial class DockingViewManager
    {
        partial class DocumentManagerImpl
        {
            class OpenDocumentTask : ITask
            {
                private readonly DocumentManagerImpl _documentManager;
                private readonly Uri _uri;
                private readonly Guid _guid;
                private readonly string _persistentInfo;

                private ICreateDocumentTask _createDocumentTask;
                public DocumentInfo DocumentInfo { get; private set; }

                public string Name
                {
                    get { return "openDocument:" + _uri.ToString(); }
                }

                public OpenDocumentTask(DocumentManagerImpl documentManager, Uri uri, Guid guid, string persistentInfo)
                {
                    _documentManager = documentManager;
                    _uri = uri;
                    _guid = guid;
                    _persistentInfo = persistentInfo;
                }

                public void Initialize(ITaskNode taskScope)
                {
                   
                }

                public void PreProcess(IProgressScope progress)
                {
                    progress.ReportStatusMessage(this.L("document", "status_opening_document", _uri.ToString()));
                    this.Run(progress);
                }

                public void PostProcess(IProgressScope progress)
                {
                    

                }

                private void Run(IProgressScope progress)
                {
                    var service = DocumentServiceManager.Instance.GetDocumentServiceOrNotifyMissing(_uri.Scheme);
                    if (service == null)
                        return;

                    var existedDocument = _documentManager.Documents.FirstOrDefault(d => d.Uri == _uri);

                    if (existedDocument != null)
                    {
                        _documentManager.SelectDocument(existedDocument);
                        this.DocumentInfo = existedDocument;
                        return;
                    }

                    _createDocumentTask = service.CreateCreateDocumentTask(_uri, _guid, _persistentInfo);

                    if (_createDocumentTask == null)
                    {
                        this.LogWarning("create document failed for '{0}' (Guid='{1}'), unable to create the document creation task", _uri, _guid);
                        return;
                    }

                    _createDocumentTask.RunSynchronized(progress);

                    if (this.DocumentInfo != null)
                        return;

                    this.DocumentInfo = _createDocumentTask.DocumentInfo;
                    if (this.DocumentInfo == null)
                    {
                        this.LogWarning("create document failed for '{0}' (Guid='{1}')", _uri, _guid);
                        return;
                    }

                    _documentManager.OpenDocument(this.DocumentInfo);
                }


                public void RunSynchronized(IProgressScope progress)
                {
                    this.Run(progress);
                }
            }
        }
    }
}

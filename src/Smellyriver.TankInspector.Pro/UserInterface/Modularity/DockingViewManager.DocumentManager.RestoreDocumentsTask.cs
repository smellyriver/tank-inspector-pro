using System.Collections.Generic;
using System.IO;
using Smellyriver.TankInspector.Common.Serialization;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Tasks;

namespace Smellyriver.TankInspector.Pro.UserInterface.Modularity
{
    partial class DockingViewManager
    {
        partial class DocumentManagerImpl
        {
            internal class RestoreDocumentsTask : CompositeTask
            {

                private readonly DocumentManagerImpl _documentManager;

                public override string Name
                {
                    get { return "restoreDocuments"; }
                }

                public RestoreDocumentsTask(DocumentManagerImpl documentManager)
                {
                    _documentManager = documentManager;
                }

                public override void PreProcess(IProgressScope progress)
                {
                    base.PreProcess(progress);
                    progress.ReportStatusMessage(this.L("document", "status_loading_documents"));
                }

                protected override IEnumerable<TaskInfo> GetChildren()
                {
                    if (File.Exists(s_documentRestorationConfigFile))
                    {

                        _documentManager.IsRestoringDocument = true;

                        var documentRestorationInfoArray = Serializer.DataContractDeserialize<DocumentRestorationInfo[]>(s_documentRestorationConfigFile);

                        foreach (var restorationInfo in documentRestorationInfoArray)
                        {
                            yield return new TaskInfo(new OpenDocumentTask(_documentManager,
                                                                           restorationInfo.Uri,
                                                                           restorationInfo.Guid,
                                                                           restorationInfo.PersistentInfo), 1.0);
                        }

                        yield return TaskInfo.FromAction("onDocumentsRestored", 0.1, p =>
                            {
                                _documentManager.IsRestoringDocument = false;
                                _documentManager.OnDocumentsRestored();
                            });
                    }
                }

            }
        }
    }
}

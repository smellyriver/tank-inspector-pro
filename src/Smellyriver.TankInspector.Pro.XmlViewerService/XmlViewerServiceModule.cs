using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Input;
using Smellyriver.TankInspector.Pro.Modularity.Modules;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;

namespace Smellyriver.TankInspector.Pro.XmlViewerService
{
    [ModuleExport("XmlViewerService", typeof(XmlViewerServiceModule))]
    [ExportMetadata("Guid", "C229BE91-98D2-479C-9199-19EFF9962EC3")]
    [ExportMetadata("Name", "#xml_viewer_service:module_name")]
    [ExportMetadata("Description", "#xml_viewer_service:module_description")]
    [ExportMetadata("Version", "1.0.0.0")]
    [ExportMetadata("Provider", "#xml_viewer_service:module_provider")]
    public class XmlViewerServiceModule : ModuleBase
    {

        public override void Initialize()
        {
            FileDocumentService.Instance.RegisterViewer(new XmlViewerService(this));

            TankCommandManager.Instance.Register(new TankCommand(guid: XmlViewerConstants.ViewTankXmlCommandGuid,
                                                                 name: this.L("xml_viewer_service", "view_xml_menu_item"), 
                                                                 execute: this.ViewXML, 
                                                                 priority: XmlViewerConstants.ViewTankXmlCommandPriority, 
                                                                 icon: BitmapImageEx.LoadAsFrozen("Resources/Images/ViewXML_16.png")));

            if (XmlViewerServiceSettings.Default.DeveloperMode)
            {
                TankCommandManager.Instance.Register(new TankCommand(guid: XmlViewerConstants.ViewTankInstanceXmlSnapshotCommandGuid,
                                                                     name: this.L("xml_viewer_service", "view_instance_xml_menu_item"), 
                                                                     execute: this.ViewInstanceXMLSnapshot, 
                                                                     priority: XmlViewerConstants.ViewTankInstanceXmlSnapshotCommandPriority, 
                                                                     icon: BitmapImageEx.LoadAsFrozen("Resources/Images/ViewXML_16.png")));
            }
        }


        private void ViewInstanceXMLSnapshot(TankUnikey key)
        {
            var repository = RepositoryManager.Instance[key.RepositoryID];
            var tank = repository.TankDatabase.Query("tank[@key = '{0}' and nation/@key = '{1}']", key.TankKey, key.NationKey);

            var uri = FileDocumentService.Instance
                          .CreateTemporaryStreamUri("tankinstance",
                                                    "xml",
                                                    key.ToString(),
                                                    TankInstanceManager.GetInstance(key).Xml.ToStream(),
                                                    this.L("xml_viewer_service", "instance_xml_document_title", tank["userString"]),
                                                    null,
                                                    RepositoryManager.Instance[key.RepositoryID]);

            DockingViewManager.Instance.DocumentManager.OpenDocument(uri);
        }

        private void ViewXML(TankUnikey key)
        {

            var repository = RepositoryManager.Instance[key.RepositoryID];
            var tank = repository.TankDatabase.Query("tank[@key = '{0}' and nation/@key = '{1}']", key.TankKey, key.NationKey);

            var uri = FileDocumentService.Instance
                          .CreateTemporaryStreamUri("tank",
                                                    "xml",
                                                    key.ToString(),
                                                    tank.Xml.ToStream(),
                                                    this.L("xml_viewer_service", "document_title", tank["userString"]),
                                                    null,
                                                    repository);

            DockingViewManager.Instance.DocumentManager.OpenDocument(uri);
        }

    }
}

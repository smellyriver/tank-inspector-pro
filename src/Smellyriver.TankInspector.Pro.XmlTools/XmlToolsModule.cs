using System;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Features;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;

namespace Smellyriver.TankInspector.Pro.XmlTools
{
    [ModuleExport("XmlTools", typeof(XmlToolsModule))]
    [ExportMetadata("Guid", "335481A3-056B-403C-8878-60466F6C5E83")]
    [ExportMetadata("Name", "#xml_tools:module_name")]
    [ExportMetadata("Description", "#xml_tools:module_description")]
    [ExportMetadata("Version", "1.0.0.0")]
    [ExportMetadata("Provider", "#xml_tools:module_provider")]
    public class XmlToolsModule : ModuleBase
    {
        private XmlToolsView _xmlToolsView;
        private XmlToolsVM _xmlToolsVM;

        public override void Initialize()
        {
            _xmlToolsView = new XmlToolsView();
            _xmlToolsVM = new XmlToolsVM(this);
            _xmlToolsView.ViewModel = _xmlToolsVM;

            var panel = new FeaturedPanelInfo(
                Guid.Parse("3DDAF122-D17F-4866-AA22-0EC514218AC6"),
                new[] { typeof(IXmlViewer) },
                this.OnRequiredFeaturesSatisficationChanged)
            {
                Title = this.L("xml_tools", "panel_title"),
                CanHide = true,
                CanClose = true,
                CanFloat = true,
                Width = 200,
                Content = _xmlToolsView,
                IconSource = BitmapImageEx.LoadAsFrozen("Resources/Images/XmlTools_16.png"),
            };

            DockingViewManager.Instance.PanelManager.Register(panel);
        }

        private void OnRequiredFeaturesSatisficationChanged(DocumentInfo document, bool satisified)
        {
            _xmlToolsView.IsEnabled = satisified;
            if (satisified)
                _xmlToolsVM.XmlViewer = document.GetFeature<IXmlViewer>();
        }
    }
}

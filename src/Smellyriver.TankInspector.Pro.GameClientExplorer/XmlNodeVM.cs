using System.Windows.Media;
using Smellyriver.TankInspector.Pro.Modularity.Modules;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.Common.Wpf.Input;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.TreeView;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer
{
    class XmlNodeVM : ExplorerTreeNodeVM
    {
        public override ImageSource IconSource
        {
            get { return NodeIconService.Current.GetNodeIcon(NodeTypes.XML); }
        }

        public string XmlContent { get; }

        private readonly string _description;
        public override string Description { get { return _description; } }

        private readonly string _category;
        private readonly string _path;

        public XmlNodeVM(ExplorerTreeNodeVM parent, string name, string category, string path, string xmlContent, string description)
            : base(parent, name, LoadChildenStrategy.Manual)
        {
            _category = category;
            _path = path;
            this.DefaultCommand = new RelayCommand(this.ViewXmlContent);
            this.XmlContent = xmlContent;
            _description = description;
        }

        private void ViewXmlContent()
        {
            var uri = FileDocumentService.Instance
                          .CreateTemporaryStreamUri(_category,
                                                    "xml",
                                                    _path,
                                                    this.XmlContent.ToStream(),
                                                    this.Name,
                                                    this.Description,
                                                    this.GameClientRoot.Model);

            DockingViewManager.Instance.DocumentManager.OpenDocument(uri);
        }
    }
}

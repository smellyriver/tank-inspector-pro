using System.Windows.Media;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.TreeView;
using Smellyriver.TankInspector.Pro.Globalization;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer
{
    class DataFolderVM : ExplorerTreeNodeVM
    {
        public override ImageSource IconSource
        {
            get { return NodeIconService.Current.GetNodeIcon(NodeTypes.DataFolder); }
        }

        public DataFolderVM(LocalGameClientNodeVM parent, LocalGameClient client)
            : base(parent, 
                   Localization.Instance.L("game_client_explorer", "data_folder"), 
                   LoadChildenStrategy.Manual)
        {
            var tanksXmlNode = new XmlNodeVM(this,
                                             this.L("game_client_explorer", "tanks_data_node"),
                                             "tanks",
                                             this.GameClientRoot.Model.ID,
                                             this.GameClientRoot.Model.TankDatabase.Xml,
                                             this.L("game_client_explorer", "Unified Tank XML Data of {0}", this.GameClientRoot.Model.RootPath));
            this.InternalChildren.Add(tanksXmlNode);

            var equipmentsXmlNode = new XmlNodeVM(this,
                                                  this.L("game_client_explorer", "equipment_data_node"),
                                                  "equipments",
                                                  this.GameClientRoot.Model.ID,
                                                  this.GameClientRoot.Model.EquipmentDatabase.Xml,
                                                  this.L("game_client_explorer", "Equipments XML Data of {0}", this.GameClientRoot.Model.RootPath));
            this.InternalChildren.Add(equipmentsXmlNode);

            var consumablesXmlNode = new XmlNodeVM(this,
                                                   this.L("game_client_explorer", "consumables_data_node"),
                                                   "consumables", 
                                                   this.GameClientRoot.Model.ID,
                                                   this.GameClientRoot.Model.ConsumableDatabase.Xml,
                                                   this.L("game_client_explorer", "Consumables XML Data of {0}", this.GameClientRoot.Model.RootPath));
            this.InternalChildren.Add(consumablesXmlNode);


            var crewsXmlNode = new XmlNodeVM(this,
                                             this.L("game_client_explorer", "crews_data_node"),
                                             "crews", 
                                             this.GameClientRoot.Model.ID,
                                             this.GameClientRoot.Model.CrewDatabase.Xml,
                                             this.L("game_client_explorer", "Crews XML Data of {0}", this.GameClientRoot.Model.RootPath));
            this.InternalChildren.Add(crewsXmlNode);

            var techTreeLayoutXmlNode = new XmlNodeVM(this,
                                                     this.L("game_client_explorer", "tech_tree_layouts_data_node"),
                                                     "layouts",
                                                     this.GameClientRoot.Model.ID,
                                                     this.GameClientRoot.Model.TechTreeLayoutDatabase.Xml,
                                                     this.L("game_client_explorer", "Tech Tree Layouts XML Data of {0}", this.GameClientRoot.Model.RootPath));
            this.InternalChildren.Add(techTreeLayoutXmlNode);
        }

    }
}

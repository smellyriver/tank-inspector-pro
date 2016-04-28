using System.Collections.Generic;
using System.Windows.Media;
using Smellyriver.TankInspector.Pro.Globalization;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.TreeView;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer.Vehicles
{
    class VehiclesFolderVM : VehicleNodeVMBase
    {
        public override ImageSource IconSource
        {
            get { return NodeIconService.Current.GetNodeIcon(NodeTypes.VehiclesFolder); }
        }

        private readonly LocalGameClient _client;

        public VehiclesFolderVM(LocalGameClientNodeVM parent, LocalGameClient client)
            : base(parent,
                   Localization.Instance.L("game_client_explorer", "tanks_folder"), 
                   LoadChildenStrategy.LazyStatic)
        {
            _client = client;
        }

        protected override IEnumerable<TreeNodeVM> LoadChildren()
        {
            foreach (var nation in _client.Nations)
            {
                yield return new NationNodeVM(this, _client, nation);
            }
        }
    }
}

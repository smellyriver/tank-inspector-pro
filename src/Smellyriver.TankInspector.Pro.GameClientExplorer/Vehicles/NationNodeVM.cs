using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Modularity.Input;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.TreeView;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer.Vehicles
{
    class NationNodeVM : VehicleNodeVMBase
    {

        private static IOrderedEnumerable<Tank> Sort(IEnumerable<Tank> tanks)
        {
            var settings = GameClientExplorerSettings.Default;
            switch ((VehicleSortingRule)settings.SortTanksBy)
            {
                case VehicleSortingRule.Class:
                    {
                        Func<Tank, int> selector = t => TankClassComparer.GetClassSortIndex(t.ClassKey);
                        if (settings.SortTanksDescending)
                            return tanks.OrderByDescending(selector);
                        else
                            return tanks.OrderBy(selector);
                    }
                case VehicleSortingRule.Name:
                    {
                        Func<Tank, string> selector = t => t.Name;
                        if (settings.SortTanksDescending)
                            return tanks.OrderByDescending(selector);
                        else
                            return tanks.OrderBy(selector);
                    }
                case VehicleSortingRule.Tier:
                    {
                        Func<Tank, int> selector = t => t.Tier;
                        if (settings.SortTanksDescending)
                            return tanks.OrderByDescending(selector);
                        else
                            return tanks.OrderBy(selector);
                    }
                default:
                    throw new NotSupportedException();
            }

        }


        private readonly LocalGameClient _client;

        public string Nation { get; }

        private readonly NationUnikey _unikey;

        public override ImageSource IconSource
        {
            get { return _client.PackageImages.GetNationSmallIcon(this.Nation); }
        }

        public NationNodeVM(ExplorerTreeNodeVM parent, LocalGameClient client, string nation)
            : base(parent, client.Localization.GetLocalizedNationName(nation), LoadChildenStrategy.LazyDynamicAsync)
        {
            _client = client;
            this.Nation = nation;
            _unikey = new NationUnikey(client, nation);
        }


        protected override List<ExplorerTreeContextMenuItemVM> CreateContextMenuItems()
        {
            var list = base.CreateContextMenuItems();

            foreach (var command in NationCommandManager.Instance.Commands)
            {
                list.Add(new ExplorerTreeContextMenuItemVM(command.Priority,
                                                           command.Name,
                                                           command,
                                                           _unikey,
                                                           command.Icon));
            }

            return list;
        }

        protected override void OnVehicleSortingRuleChanged()
        {
            this.Refresh();
        }


        protected override IEnumerable<TreeNodeVM> LoadChildren()
        {
            var tankElements = _client.TankDatabase.QueryMany("tank[nation/@key = '{0}']", this.Nation);
            foreach (var tank in NationNodeVM.Sort(tankElements.Select(t => Tank.Create(t))))
            {
                yield return new TankNodeVM(this, tank, _client);
            }
        }

    }
}

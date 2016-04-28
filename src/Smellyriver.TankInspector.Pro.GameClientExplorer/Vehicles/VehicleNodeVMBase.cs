using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Smellyriver.TankInspector.Pro.Globalization;
using Smellyriver.TankInspector.Common.Wpf.Input;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.Menu;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.TreeView;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer.Vehicles
{
    abstract class VehicleNodeVMBase : ExplorerTreeNodeVM
    {
        private static ExplorerTreeContextMenuItemVM s_sortByMenu;
        private static MenuItemVM[] s_sortByMenuItems;

        private static event EventHandler VehicleSortingRuleChanged;

        protected static VehicleSortingRule VehicleSortingRule
        {
            get { return (VehicleSortingRule)GameClientExplorerSettings.Default.SortTanksBy; }
        }

        protected static bool DescendingSorting
        {
            get { return GameClientExplorerSettings.Default.SortTanksDescending; }
        }

        static VehicleNodeVMBase()
        {
            s_sortByMenu = new ExplorerTreeContextMenuItemVM(100, Localization.Instance.L("game_client_explorer", "sort_tanks_by_menu_item"), null);
            s_sortByMenu.IsDefault = false;

            s_sortByMenu.MenuItems.Add(VehicleNodeVMBase.CreateSortMenuItem(VehicleSortingRule.Name));
            s_sortByMenu.MenuItems.Add(VehicleNodeVMBase.CreateSortMenuItem(VehicleSortingRule.Tier));
            s_sortByMenu.MenuItems.Add(VehicleNodeVMBase.CreateSortMenuItem(VehicleSortingRule.Class));

            s_sortByMenuItems = s_sortByMenu.MenuItems.ToArray();

            VehicleNodeVMBase.UpdateSortItemStates();

            GameClientExplorerSettings.Default.PropertyChanged += VehicleNodeVMBase.GameClientExplorerSettings_PropertyChanged;
        }

        static void GameClientExplorerSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SortTanksBy" || e.PropertyName == "SortTanksDescending")
            {
                VehicleNodeVMBase.UpdateSortItemStates();

                if (VehicleNodeVMBase.VehicleSortingRuleChanged != null)
                    VehicleNodeVMBase.VehicleSortingRuleChanged(null, EventArgs.Empty);
            }
        }

        private static void UpdateSortItemStates()
        {
            for (var i = 0; i < s_sortByMenuItems.Length; ++i)
            {
                s_sortByMenuItems[i].IsChecked = i == GameClientExplorerSettings.Default.SortTanksBy;
            }
        }

        private static ExplorerTreeContextMenuItemVM CreateSortMenuItem(VehicleSortingRule sortBy)
        {
            string name;
            switch(sortBy)
            {
                case VehicleSortingRule.Class:
                    name = Localization.Instance.L("game_client_explorer", "sort_by_class_menu_item");
                    break;
                case VehicleSortingRule.Name:
                    name = Localization.Instance.L("game_client_explorer", "sort_by_name_menu_item");
                    break;
                case VehicleSortingRule.Tier:
                    name = Localization.Instance.L("game_client_explorer", "sort_by_tier_menu_item");
                    break;
                default:
                    throw new ArgumentException("sortBy");
            }
            var menuItem = new ExplorerTreeContextMenuItemVM(0, name, new RelayCommand(() => VehicleNodeVMBase.SortBy(sortBy)));
            menuItem.IsCheckable = true;
            menuItem.IsChecked = false;
            return menuItem;
        }

        private static void SortBy(VehicleSortingRule sortBy)
        {
            var settings = GameClientExplorerSettings.Default;
            if (VehicleNodeVMBase.VehicleSortingRule == sortBy)
                settings.SortTanksDescending = !settings.SortTanksDescending;
            else
                settings.SortTanksBy = (int)sortBy;

            settings.Save();
        }

        public VehicleNodeVMBase(ExplorerTreeNodeVM parent, string name, LoadChildenStrategy loadChildrenStrategy)
            : base(parent, name, loadChildrenStrategy)
        {
            VehicleNodeVMBase.VehicleSortingRuleChanged += OnVehicleSortingRuleChanged;
        }

        void OnVehicleSortingRuleChanged(object sender, EventArgs e)
        {
            if (!this.IsExpanded)
                return;

            this.OnVehicleSortingRuleChanged();
        }

        protected virtual void OnVehicleSortingRuleChanged()
        {

        }

        protected override List<ExplorerTreeContextMenuItemVM> CreateContextMenuItems()
        {
            var list = base.CreateContextMenuItems();
            list.Add(s_sortByMenu);

            return list;
        }
    }
}

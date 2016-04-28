using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.Menu;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.Pro.Globalization;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Menus;

namespace Smellyriver.TankInspector.Pro.ModelShared
{
    public static class Menu
    {
        private static bool _sharedMenuItemsRegistered;

        public static void RegisterSharedMenuItems()
        {
            if (_sharedMenuItemsRegistered)
                return;

            MenuManager.Instance.Register(new MenuItemVM(Localization.Instance.L("model_shared", "snapshot_menu_item"), Commands.Snapshot)
            {
                Icon = BitmapImageEx.LoadAsFrozen("Resources/Images/Snapshot_16.png")
            }, MenuAnchor.Export);

            MenuManager.Instance.Register(new MenuItemVM(Localization.Instance.L("model_shared", "solid_mode"), Commands.SwitchToSolidMode)
            {
                IsCheckable = true,
                IsChecked = true,
            }, 
            MenuAnchor.View, 
            Commands.WireframeGroupPriority);

            MenuManager.Instance.Register(new MenuItemVM(Localization.Instance.L("model_shared", "wireframe_mode"), Commands.SwitchToWireframeMode)
            {
                IsCheckable = true,
            },
            MenuAnchor.View,
            Commands.WireframeGroupPriority);

            MenuManager.Instance.Register(new MenuItemVM(Localization.Instance.L("model_shared", "toggle_show_turret_menu_item"), Commands.ToggleShowTurret)
            {
                IsCheckable = true,
                IsChecked = true,
            },
            MenuAnchor.View,
            Commands.ShowModulesGroupPriority);

            MenuManager.Instance.Register(new MenuItemVM(Localization.Instance.L("model_shared", "toggle_show_gun_menu_item"), Commands.ToggleShowGun)
            {
                IsCheckable = true,
                IsChecked = true,
            },
            MenuAnchor.View,
            Commands.ShowModulesGroupPriority);

            MenuManager.Instance.Register(new MenuItemVM(Localization.Instance.L("model_shared", "toggle_show_hull_menu_item"), Commands.ToggleShowHull)
            {
                IsCheckable = true,
                IsChecked = true,
            },
            MenuAnchor.View,
            Commands.ShowModulesGroupPriority);

            MenuManager.Instance.Register(new MenuItemVM(Localization.Instance.L("model_shared", "toggle_show_chassis_menu_item"), Commands.ToggleShowChassis)
            {
                IsCheckable = true,
                IsChecked = true,
            },
            MenuAnchor.View,
            Commands.ShowModulesGroupPriority);

            MenuManager.Instance.Register(new MenuItemVM(Localization.Instance.L("model_shared", "toggle_show_triangle_count_menu_item"), Commands.ToggleTriangleCountDisplay)
            {
                IsCheckable = true,
            },
            MenuAnchor.View,
            Commands.ShowFPSGroupPriority);

            MenuManager.Instance.Register(new MenuItemVM(Localization.Instance.L("model_shared", "toggle_show_fps_menu_item"), Commands.ToggleFPSDisplay)
            {
                IsCheckable = true,
            },
            MenuAnchor.View,
            Commands.ShowFPSGroupPriority);

            _sharedMenuItemsRegistered = true;
        }
    }
}

using System.Windows.Input;
using Smellyriver.TankInspector.Pro.Globalization;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    static class Commands
    {
        public static readonly RoutedUICommand NewComparison 
            = new RoutedUICommand(Localization.Instance.L("stat_comparer", "new_stat_comparison_menu_item"), "newComparison", typeof(Commands));

        public static readonly RoutedUICommand ToggleConfigPanel
            = new RoutedUICommand(Localization.Instance.L("stat_comparer", "configuration_panel_menu_item"), "toggleConfigPanel", typeof(Commands));

        public static readonly RoutedUICommand SelectInstanceValueMode
            = new RoutedUICommand(Localization.Instance.L("stat_comparer", "instance_values_menu_item"), "selectInstanceValueMode", typeof(Commands));

        public static readonly RoutedUICommand SelectTanksColumnMode
            = new RoutedUICommand(Localization.Instance.L("stat_comparer", "tanks_as_columns_menu_item"), "selectTanksColumnMode", typeof(Commands));

        public static readonly RoutedUICommand SelectStatsColumnMode
            = new RoutedUICommand(Localization.Instance.L("stat_comparer", "stats_as_columns_menu_item"), "selectStatsColumnMode", typeof(Commands));

        public static readonly RoutedUICommand SelectBaseValueMode
            = new RoutedUICommand(Localization.Instance.L("stat_comparer", "base_values_menu_item"), "selectBaseValueMode", typeof(Commands));


        public static readonly RoutedUICommand ExportCsv
            = new RoutedUICommand(Localization.Instance.L("stat_comparer", "export_to_csv_sheet_menu_item"), "exportCsvSheet", typeof(Commands));
    }
}

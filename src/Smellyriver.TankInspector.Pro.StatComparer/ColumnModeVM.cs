using Smellyriver.TankInspector.Pro.Globalization;

namespace Smellyriver.TankInspector.Pro.StatComparer
{

    class ColumnModeVM
    {
        public static ColumnModeVM[] ColumnModes
        {
            get
            {
                return new[]
                {
                    new ColumnModeVM(ColumnMode.Stats, 
                                     Localization.Instance.L("stat_comparer", "column_mode_stats"), 
                                     Localization.Instance.L("stat_comparer", "column_mode_stats_description")),
                    new ColumnModeVM(ColumnMode.Tanks, 
                                     Localization.Instance.L("stat_comparer", "column_mode_tanks"), 
                                     Localization.Instance.L("stat_comparer", "column_mode_tanks_description")),
                };
            }
        }

        public ColumnMode Mode { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        public ColumnModeVM(ColumnMode mode, string name, string description)
        {
            this.Mode = mode;
            this.Name = name;
            this.Description = description;
        }
    }
}

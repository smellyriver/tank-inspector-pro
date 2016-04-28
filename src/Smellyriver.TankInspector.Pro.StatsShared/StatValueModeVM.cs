using System.Windows.Media;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.Pro.Globalization;

namespace Smellyriver.TankInspector.Pro.Data.Stats
{
    public class StatValueModeVM
    {
        public static StatValueModeVM[] StatValueModes
        {
            get
            {
                return new[]
                {
                    new StatValueModeVM(StatValueMode.Base, 
                                        Localization.Instance.L("stats_shared", "stat_value_mode_base"), 
                                        Localization.Instance.L("stats_shared", "stat_value_mode_base_description"), 
                                        BitmapImageEx.LoadAsFrozen("Resources/Images/BaseValue_16.png")),
                    new StatValueModeVM(StatValueMode.Instance, 
                                        Localization.Instance.L("stats_shared", "stat_value_mode_instance"), 
                                        Localization.Instance.L("stats_shared", "stat_value_mode_instance_description"), 
                                        BitmapImageEx.LoadAsFrozen("Resources/Images/InstanceValue_16.png")),
                };
            }
        }

        public StatValueMode Mode { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public ImageSource Icon { get; private set; }

        public StatValueModeVM(StatValueMode mode, string name, string description, ImageSource icon)
        {
            this.Mode = mode;
            this.Name = name;
            this.Description = description;
            this.Icon = icon;
        }
    }
}

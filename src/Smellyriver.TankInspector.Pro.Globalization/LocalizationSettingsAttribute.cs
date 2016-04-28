using System;

namespace Smellyriver.TankInspector.Pro.Globalization
{
    public class LocalizationSettingsAttribute : Attribute
    {
        public const string DefaultLocalizationDirectory = "Localization";

        public string LocalizationDirectory { get; set; }
        public bool FallbackToMainAssembly { get; set; }

        public LocalizationSettingsAttribute()
        {
            this.LocalizationDirectory = DefaultLocalizationDirectory;
            this.FallbackToMainAssembly = true;
        }

    }
}

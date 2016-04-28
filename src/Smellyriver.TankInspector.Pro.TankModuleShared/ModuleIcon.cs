using System;
using System.Collections.Generic;
using System.Windows.Media;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.Pro.Data.Entities;

namespace Smellyriver.TankInspector.Pro.TankModuleShared
{
    public static class ModuleIcon
    {
        private static readonly Dictionary<Type, ImageSource> s_moduleIcons =
            new Dictionary<Type, ImageSource>()
            {
                { typeof(Gun), BitmapImageEx.LoadAsFrozen("Resources/Images/Modules/Gun_64.png") },
                { typeof(Turret), BitmapImageEx.LoadAsFrozen("Resources/Images/Modules/Turret_64.png") },
                { typeof(Chassis), BitmapImageEx.LoadAsFrozen("Resources/Images/Modules/Chassis_64.png") },
                { typeof(Engine), BitmapImageEx.LoadAsFrozen("Resources/Images/Modules/Engine_64.png") },
                { typeof(Radio), BitmapImageEx.LoadAsFrozen("Resources/Images/Modules/Radio_64.png") },
            };

        public static ImageSource GetModuleIcon(Type moduleType)
        {
            ImageSource icon;

            if (s_moduleIcons.TryGetValue(moduleType, out icon))
                return icon;

            return null;
        }

        public static ImageSource GetModuleIcon(string moduleType)
        {
            Type type;
            if (Module.TryParseModuleType(moduleType, out type))
                return ModuleIcon.GetModuleIcon(type);

            return null;
        }
    }
}

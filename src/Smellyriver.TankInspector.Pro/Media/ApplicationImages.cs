using System;
using System.Windows.Media;
using Smellyriver.TankInspector.Common.Wpf.Utilities;

namespace Smellyriver.TankInspector.Pro.Media
{
    public static class ApplicationImages
    {

        public static readonly ImageSource LightTankIcon16 = BitmapImageEx.LoadAsFrozen("Resources/Images/Gameplay/lightTank_16.png");
        public static readonly ImageSource HeavyTankIcon16 = BitmapImageEx.LoadAsFrozen("Resources/Images/Gameplay/heavyTank_16.png");
        public static readonly ImageSource MediumTankIcon16 = BitmapImageEx.LoadAsFrozen("Resources/Images/Gameplay/mediumTank_16.png");
        public static readonly ImageSource SPGIcon16 = BitmapImageEx.LoadAsFrozen("Resources/Images/Gameplay/SPG_16.png");
        public static readonly ImageSource ATSPGIcon16 = BitmapImageEx.LoadAsFrozen("Resources/Images/Gameplay/AT-SPG_16.png");

        public static ImageSource GetTankClassIcon(string tankClass)
        {
            var icon = ApplicationImages.TryGetTankClassIcon(tankClass);
            if (icon == null)
                throw new ArgumentException("tankClass", "invalid tank class");

            return icon;
        }

        public static ImageSource TryGetTankClassIcon(string tankClass)
        {
            switch (tankClass)
            {
                case "lightTank":
                    return LightTankIcon16;
                case "mediumTank":
                    return MediumTankIcon16;
                case "heavyTank":
                    return HeavyTankIcon16;
                case "AT-SPG":
                    return ATSPGIcon16;
                case "SPG":
                    return SPGIcon16;
                default:
                    return null;
            }
        }
    }
}

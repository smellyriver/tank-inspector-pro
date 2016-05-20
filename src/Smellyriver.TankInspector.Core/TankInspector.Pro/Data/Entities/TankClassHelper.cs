using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public static class TankClassHelper
    {
        public const string ObserverClassKey = "observer";
        public const string LightTankClassKey = "lightTank";
        public const string MediumTankClassKey = "mediumTank";
        public const string HeavyTankClassKey = "heavyTank";
        public const string ATSPGClassKey = "AT-SPG";
        public const string SPGClassKey = "SPG";

        public static readonly string[] AllTankClassKeys = new[]
        {
            ObserverClassKey,
            LightTankClassKey,
            MediumTankClassKey,
            HeavyTankClassKey,
            ATSPGClassKey,
            SPGClassKey
        };

        public static readonly string[] RegularTankClassKeys = new[]
        {
            LightTankClassKey,
            MediumTankClassKey,
            HeavyTankClassKey,
            ATSPGClassKey,
            SPGClassKey
        };


        public static TankClass FromClassKey(string type)
        {
            switch (type)
            {
                case TankClassHelper.ObserverClassKey:
                    return TankClass.Observer;
                case TankClassHelper.LightTankClassKey:
                    return TankClass.LightTank;
                case TankClassHelper.MediumTankClassKey:
                    return TankClass.MediumTank;
                case TankClassHelper.HeavyTankClassKey:
                    return TankClass.HeavyTank;
                case TankClassHelper.ATSPGClassKey:
                    return TankClass.ATSPG;
                case TankClassHelper.SPGClassKey:
                    return TankClass.SPG;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }
    }
}

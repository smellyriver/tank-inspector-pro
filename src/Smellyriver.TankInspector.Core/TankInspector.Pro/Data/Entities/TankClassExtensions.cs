using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public static class TankClassExtensions
    {
        public static string ToClassKey(this TankClass type)
        {
            switch (type)
            {
                case TankClass.Observer:
                    return TankClassHelper.ObserverClassKey;
                case TankClass.LightTank:
                    return TankClassHelper.LightTankClassKey;
                case TankClass.MediumTank:
                    return TankClassHelper.MediumTankClassKey;
                case TankClass.HeavyTank:
                    return TankClassHelper.HeavyTankClassKey;
                case TankClass.ATSPG:
                    return TankClassHelper.ATSPGClassKey;
                case TankClass.SPG:
                    return TankClassHelper.SPGClassKey;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }
    }
}

using System;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{

    public static class ShellTypeExtensions
    {
        public static string ToXmlValue(this ShellType type)
        {
            switch (type)
            {
                case ShellType.HE:
                    return ShellTypeHelper.HEXmlName;
                case ShellType.PremiumHE:
                    return ShellTypeHelper.PremiumHEXmlName;
                case ShellType.HEAT:
                    return ShellTypeHelper.HEATXmlName;
                case ShellType.AP:
                    return ShellTypeHelper.APXmlName;
                case ShellType.APHE:
                    return ShellTypeHelper.APHEXmlName;
                case ShellType.APCR:
                    return ShellTypeHelper.APCRXmlName;
                default:
                    throw new NotSupportedException();
            }
        }

        public static bool IsKineticShellType(this ShellType type)
        {
            switch (type)
            {
                case ShellType.HE:
                case ShellType.PremiumHE:
                    return false;
                case ShellType.AP:
                case ShellType.APCR:
                case ShellType.APHE:
                case ShellType.HEAT:
                    return true;
                default:
                    throw new NotSupportedException();
            }
        }

        public static bool HasNormalizationEffect(this ShellType type)
        {
            switch (type)
            {
                case ShellType.HE:
                case ShellType.PremiumHE:
                case ShellType.HEAT:
                    return false;
                case ShellType.AP:
                case ShellType.APCR:
                case ShellType.APHE:
                    return true;
                default:
                    throw new NotSupportedException();
            }
        }

        public static double GetBaseNormalization(this ShellType type)
        {
            switch (type)
            {
                case ShellType.HE:
                case ShellType.PremiumHE:
                case ShellType.HEAT:
                    return 0.0;
                case ShellType.AP:
                case ShellType.APHE:
                    return 5.0;
                case ShellType.APCR:
                    return 2.0;
                default:
                    throw new NotSupportedException();
            }
        }

        public static double GetRicochetAngle(this ShellType type)
        {
            switch (type)
            {
                case ShellType.HE:
                case ShellType.PremiumHE:
                    return 90.0;
                case ShellType.HEAT:
                    return 85.0;
                case ShellType.AP:
                case ShellType.APHE:
                case ShellType.APCR:
                    return 70.0;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}

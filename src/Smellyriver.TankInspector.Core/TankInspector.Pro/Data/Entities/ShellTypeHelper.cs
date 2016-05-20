using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public static class ShellTypeHelper
    {
        public const string HEXmlName = "HIGH_EXPLOSIVE";
        public const string PremiumHEXmlName = "HIGH_EXPLOSIVE_PREMIUM";
        public const string HEATXmlName = "HOLLOW_CHARGE";
        public const string APXmlName = "ARMOR_PIERCING";
        public const string APHEXmlName = "ARMOR_PIERCING_HE";
        public const string APCRXmlName = "ARMOR_PIERCING_CR";

        public static ShellType FromXmlValue(string xmlValue)
        {

            switch (xmlValue)
            {
                case ShellTypeHelper.HEXmlName:
                    return ShellType.HE;
                case ShellTypeHelper.PremiumHEXmlName:
                    return ShellType.PremiumHE;
                case ShellTypeHelper.HEATXmlName:
                    return ShellType.HEAT;
                case ShellTypeHelper.APXmlName:
                    return ShellType.AP;
                case ShellTypeHelper.APHEXmlName:
                    return ShellType.APHE;
                case ShellTypeHelper.APCRXmlName:
                    return ShellType.APCR;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public static class NationalCustomizationDatabaseExtensions
    {

        public static Color GetArmorColor(this NationalCustomizationDatabase @this)
        {
            return CamouflageHelper.QueryColor(@this, "armorColor", 0xff).Value;
        }
    }
}

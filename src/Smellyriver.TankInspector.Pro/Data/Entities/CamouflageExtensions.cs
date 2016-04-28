using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public static class CamouflageExtensions
    {
        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        public static Color[] GetColors(this Camouflage @this)
        {
            return new Color[]
            {
                CamouflageHelper.QueryColor(@this, "colors/c0").Value,
                CamouflageHelper.QueryColor(@this, "colors/c1").Value,
                CamouflageHelper.QueryColor(@this, "colors/c2").Value,
                CamouflageHelper.QueryColor(@this, "colors/c3").Value,
            };
        }



        public static double[] GetTiling(this Camouflage @this, string tankKey)
        {
            return CamouflageHelper.QueryVector4(@this, "tiling/" + tankKey);
        }

        public static double[] GetMetallic(this Camouflage @this, string tankKey)
        {
            return CamouflageHelper.QueryVector4(@this, "metallic/" + tankKey);
        }

        public static double[] GetGloss(this Camouflage @this, string tankKey)
        {
            return CamouflageHelper.QueryVector4(@this, "gloss/" + tankKey);
        }

    }
}

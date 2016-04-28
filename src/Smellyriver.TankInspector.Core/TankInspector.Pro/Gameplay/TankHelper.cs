using System.Collections.Generic;
using Smellyriver.Collections;
using Smellyriver.TankInspector.Pro.Data;

namespace Smellyriver.TankInspector.Pro.Gameplay
{
    public static class TankHelper
    {
        public static readonly IEqualityComparer<IXQueryable> KeyEqualityComparer
               = ProjectionEqualityComparer<IXQueryable>.Create(g => g == null ? null : g["@key"]);
    }
}

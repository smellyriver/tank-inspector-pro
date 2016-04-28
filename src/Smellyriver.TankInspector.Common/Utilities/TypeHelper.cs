using System;
using System.Collections.Generic;
using System.Linq;

namespace Smellyriver.TankInspector.Common.Utilities
{
    public static class TypeHelper
    {
        public static IEnumerable<Type> GetAllLoadedTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes());
        }
    }
}

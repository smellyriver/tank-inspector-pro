using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smellyriver.Collections;
using Smellyriver.TankInspector.Pro.Data;

namespace Smellyriver.TankInspector.Pro.Gameplay
{
    public static class KeyEqualityComparer<T> where T : IXQueryable
    {
        private static IEqualityComparer<T> s_instance;
        public static IEqualityComparer<T> Instance
        {
            get { return s_instance ?? (s_instance = ProjectionEqualityComparer<T>.Create(t => t != null ? t["@key"] : null)); }
        }
    }

    public static class KeyEqualityComparer
    {
        public static bool KeyEquals<T>(this T t, T other) where T : IXQueryable
        {
            return KeyEqualityComparer<T>.Instance.Equals(t, other);
        }
    }
}

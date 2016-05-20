using System.Collections.Generic;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public sealed class TankClassComparer : IComparer<string>, IComparer<TankClass>
    {
        private static readonly Dictionary<string, int> s_classKeySortIndices
            = new Dictionary<string, int>
            {
                {TankClassHelper.ObserverClassKey, 0},
                {TankClassHelper.LightTankClassKey, 1},
                {TankClassHelper.MediumTankClassKey, 2},
                {TankClassHelper.HeavyTankClassKey, 3},
                {TankClassHelper.ATSPGClassKey, 4},
                {TankClassHelper.SPGClassKey, 5},
            };


        private const int c_unknownValue = 6;

        public static int GetClassSortIndex(string classKey)
        {
            int value;
            return s_classKeySortIndices.TryGetValue(classKey, out value) ? value : c_unknownValue;
        }

        public int Compare(string x, string y)
        {
            var xValue = TankClassComparer.GetClassSortIndex(x);
            var yValue = TankClassComparer.GetClassSortIndex(y);

            if (xValue == yValue)
                return string.Compare(x, y);

            return xValue - yValue;
        }

        public int Compare(TankClass x, TankClass y)
        {
            return (int)x - (int)y;
        }
    }
}

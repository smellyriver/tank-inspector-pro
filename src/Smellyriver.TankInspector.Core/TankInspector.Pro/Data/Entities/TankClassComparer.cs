using System.Collections.Generic;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public sealed class TankClassComparer : IComparer<string>
    {
        private static readonly Dictionary<string, int> s_classSortIndices
            = new Dictionary<string, int>
            {
                { "lightTank", 0 },
                { "mediumTank", 1 },
                { "heavyTank", 2 },
                { "AT-SPG", 3 },
                { "SPG", 4 },
            };

        private const int c_unknownValue = 5;

        public static int GetClassSortIndex(string classKey)
        {
            int value;
            if (s_classSortIndices.TryGetValue(classKey, out value))
                return value;

            return c_unknownValue;
        }

        public int Compare(string x, string y)
        {
            var xValue = TankClassComparer.GetClassSortIndex(x);
            var yValue = TankClassComparer.GetClassSortIndex(y);

            if (xValue == yValue)
                return string.Compare(x, y);

            return xValue - yValue;
        }
    }
}

using System;
using System.Threading;

namespace Smellyriver.TankInspector.Common
{
    public static class Diagnostics
    {
        private static ThreadLocal<int> _potentialExceptionRegionLevel = new ThreadLocal<int>(() => 0);

        public static IDisposable PotentialExceptionRegion
        {
            get
            {
                return new PotentialExceptionRegionHolder();
            }
        }

        public static bool IsInPotentialExceptionRegion
        {
            get
            {
                return _potentialExceptionRegionLevel.Value != 0;
            }
        }

        private class PotentialExceptionRegionHolder : IDisposable
        {
            public PotentialExceptionRegionHolder()
            {
                ++Diagnostics._potentialExceptionRegionLevel.Value;
            }
            public void Dispose()
            {
                --Diagnostics._potentialExceptionRegionLevel.Value;
            }
        }
    }
}

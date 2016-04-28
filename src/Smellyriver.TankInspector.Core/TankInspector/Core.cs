using System;

namespace Smellyriver.TankInspector
{
    public static class Core
    {
        private static ICoreSupport s_support;

        internal static ICoreSupport Support
        {
            get
            {
                if (s_support == null)
                    throw new InvalidOperationException("core module is not initialized");

                return s_support;
            }
        }

        public static void Initialize(ICoreSupport configuration)
        {
            s_support = configuration;
        }
    }
}

using System;

namespace Smellyriver.TankInspector.Common.Utilities
{
    public static class IServiceProviderExtensions
    {
        public static TService GetService<TService>(this IServiceProvider @this)
        {
            return (TService)@this.GetService(typeof(TService));
        }
    }
}

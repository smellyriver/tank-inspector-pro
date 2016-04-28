using System;
using System.Windows.Threading;

namespace Smellyriver.TankInspector.Common.Wpf.Utilities
{
    public static class DispatcherExtensions
    {
        public static T AutoInvoke<T>(this Dispatcher dispatcher, Func<T> func)
        {
            if (dispatcher.CheckAccess())
                return func();
            else
                return (T)dispatcher.Invoke(func);
        }

        public static void AutoInvoke(this Dispatcher dispatcher, Action action)
        {
            if (dispatcher.CheckAccess())
                action();
            else
                dispatcher.Invoke(action);
        }
    }
}

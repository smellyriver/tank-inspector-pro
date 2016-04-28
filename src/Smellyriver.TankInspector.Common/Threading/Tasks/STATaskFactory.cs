using System;
using System.Threading;
using System.Threading.Tasks;

namespace Smellyriver.TankInspector.Common.Threading.Tasks
{
    public static class STATaskFactory
    {
        public static Task<T> StartNew<T>(Func<T> func)
        {
            var completionSource = new TaskCompletionSource<T>();
            Thread thread = new Thread(() =>
            {
                try
                {
                    completionSource.SetResult(func());
                }
                catch (Exception e)
                {
                    completionSource.SetException(e);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return completionSource.Task;
        }

        public static Task StartNew(Action action)
        {
            var completionSource = new TaskCompletionSource<object>();
            Thread thread = new Thread(() =>
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    completionSource.SetException(e);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return completionSource.Task;
        }
    }
}

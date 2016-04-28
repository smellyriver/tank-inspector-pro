using System;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Smellyriver.TankInspector.Pro.Networking.Remoting.XmlRpc;

namespace Smellyriver.TankInspector.Pro.Networking.Remoting
{
    public abstract class RemoteServiceBase<TRemoteService>
        where TRemoteService : IRemoteService
    {
        private static readonly ILog log = SafeLog.GetLogger("RemoteServiceBase");

        protected TRemoteService Remote { get; private set; }
        TaskFactory _asyncRpcTaskFactory;
        TaskScheduler _asyncRpcScheduler;
        CancellationToken _asyncRpcCancellationToken;

        TaskScheduler _callBackScheduler;

        protected RemoteServiceBase()
        {
            this.Remote = (TRemoteService)XmlRpcProxyGen.Create(typeof(TRemoteService));
            _asyncRpcScheduler = new LimitedConcurrencyLevelTaskScheduler(1);
            _asyncRpcTaskFactory = new TaskFactory(_asyncRpcCancellationToken, TaskCreationOptions.None, TaskContinuationOptions.None, _asyncRpcScheduler);
            _asyncRpcCancellationToken = new CancellationToken();
            _callBackScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        protected void AsyncRemoteCall<R>(Func<R> remoteFunction, Action<R> callBack, Action<RemoteServiceException> callBackWhenExceptionOccur)
        {
            var task = _asyncRpcTaskFactory.StartNew(() =>
            {
                using (Diagnostics.PotentialExceptionRegion)
                {
                    return remoteFunction();
                }
            });
            BindCallBack(task, callBack, callBackWhenExceptionOccur);
        }

        protected void AsyncRemoteCall<T1, R>(Func<T1, R> remoteFunction, T1 arg, Action<R> callBack, Action<RemoteServiceException> callBackWhenExceptionOccur)
        {
            var task = _asyncRpcTaskFactory.StartNew(() =>
            {
                using (Diagnostics.PotentialExceptionRegion)
                {
                    return remoteFunction(arg);
                }
            });

            BindCallBack(task, callBack, callBackWhenExceptionOccur);
        }

        protected void AsyncRemoteCall<T1, T2, R>(Func<T1, T2, R> remoteFunction, T1 arg1, T2 arg2, Action<R> callBack, Action<RemoteServiceException> callBackWhenExceptionOccur)
        {
            var task = _asyncRpcTaskFactory.StartNew(() =>
            {
                using (Diagnostics.PotentialExceptionRegion)
                {
                    return remoteFunction(arg1, arg2);
                }
            });

            BindCallBack(task, callBack, callBackWhenExceptionOccur);
        }

        protected void AsyncRemoteCall<T1, T2, T3, R>(Func<T1, T2, T3, R> remoteFunction, T1 arg1, T2 arg2, T3 arg3, Action<R> callBack, Action<RemoteServiceException> callBackWhenExceptionOccur)
        {
            var task = _asyncRpcTaskFactory.StartNew(() =>
            {
                using (Diagnostics.PotentialExceptionRegion)
                {
                    return remoteFunction(arg1, arg2, arg3);
                }
            });

            BindCallBack(task, callBack, callBackWhenExceptionOccur);
        }

        protected void BindCallBack<R>(Task<R> task, Action<R> callBack, Action<RemoteServiceException> callBackWhenExceptionOccur)
        {
            task.ContinueWith((t) =>
            {
                if (t.IsFaulted)
                {
                    if (t.Exception != null)
                    {
                        t.Exception.Handle(x =>
                        {
                            log.WarnFormat("RPC falted", x);
                            var outter = new RemoteServiceException(x);
                            callBackWhenExceptionOccur(outter);
                            return true;
                        });
                    }
                }
                else if (t.IsCompleted)
                    callBack(t.Result);
            },
            _callBackScheduler
            );
        }
    }
}

using System;
using Smellyriver.TankInspector.Pro.Networking.Livestat;
using Smellyriver.TankInspector.Pro.Networking.Remoting;

namespace Smellyriver.TankInspector.Pro.Networking
{
    class TankInspectorRemoteService : RemoteServiceBase<ITankInspectorRemoteService>
    {
        public static TankInspectorRemoteService Instance { get; private set; }

        public void Register(string email, string password, string invitationCode, Action<RegisterResult> callback, Action<RemoteServiceException> callBackWhenExceptionOccur)
        {
            AsyncRemoteCall(this.Remote.Register, email, password, invitationCode, callback, callBackWhenExceptionOccur);
        }

        public void Login(string user, string password, Action<bool> callback, Action<RemoteServiceException> callBackWhenExceptionOccur)
        {
#if NOLOGIN
            callback(true);
#else
            AsyncRemoteCall(this.Remote.Login, user, password, callback, callBackWhenExceptionOccur);
#endif
        }

        public void LoginEx(string user, string password, int version, Action<bool> callback, Action<RemoteServiceException> callBackWhenExceptionOccur)
        {
#if NOLOGIN
            callback(true);
#else
            AsyncRemoteCall(this.Remote.LoginEx, user, password, version, callback, callBackWhenExceptionOccur);
#endif
        }

        public void LoginPro(string user, string password, int version, Action<bool> callback, Action<RemoteServiceException> callBackWhenExceptionOccur)
        {
#if NOLOGIN
            callback(true);
#else
            AsyncRemoteCall(this.Remote.LoginPro, user, password, version, callback, callBackWhenExceptionOccur);
#endif
        }

        public void Logout(Action<bool> callback, Action<RemoteServiceException> callBackWhenExceptionOccur)
        {
            AsyncRemoteCall(this.Remote.Logout, callback, callBackWhenExceptionOccur);
        }

        public void Logout()
        {
            Logout(result => { }, e => { });
        }

        public void Check(Action<CheckResult> callback, Action<RemoteServiceException> callBackWhenExceptionOccur)
        {
#if NOLOGIN
            callback(CheckResult.everything_ok);
#else
            AsyncRemoteCall(this.Remote.Check, callback, callBackWhenExceptionOccur);
#endif
        }

        public void ResendActivationEmail(Action<ResendActivationEmailResult> callback, Action<RemoteServiceException> callBackWhenExceptionOccur)
        {
            AsyncRemoteCall(this.Remote.ResendActivationEmail, callback, callBackWhenExceptionOccur);
        }

        public void SendPasswordResetEmail(string email, Action<bool> callback, Action<RemoteServiceException> callBackWhenExceptionOccur)
        {
            AsyncRemoteCall(this.Remote.SendPasswordResetEmail, email, callback, callBackWhenExceptionOccur);
        }

        public void IsServiceEnabled(string serviceName, Action<bool> callback, Action<RemoteServiceException> callBackWhenExceptionOccur)
        {
            AsyncRemoteCall(this.Remote.IsServiceEnabled, serviceName, callback, callBackWhenExceptionOccur);
        }

        public void GetNotifications(Action<Notification[]> callback, Action<RemoteServiceException> callBackWhenExceptionOccur)
        {
            AsyncRemoteCall(this.Remote.GetNotifications, callback, callBackWhenExceptionOccur);
        }

        public void GetLivestat(uint typeCompDescr, uint clientVersionNumber, bool fallBackToLiveVersion, Action<LivestatData> callback, Action<RemoteServiceException> callBackWhenExceptionOccur)
        {
            AsyncRemoteCall(this.Remote.GetLivestat, (int)typeCompDescr, (int)clientVersionNumber, fallBackToLiveVersion, callback, callBackWhenExceptionOccur);
        }

        public LivestatData GetLivestat(uint typeCompDescr, uint clientVersionNumber, bool fallBackToLiveVersion)
        {
            return this.Remote.GetLivestat((int)typeCompDescr, (int)clientVersionNumber, fallBackToLiveVersion);
        }

        public void GetAccountInfo(Action<AccountInfo> callback, Action<RemoteServiceException> callBackWhenExceptionOccur)
        {
#if NOLOGIN
            callback(new AccountInfo());
#else
            AsyncRemoteCall(this.Remote.GetAccountInfo, callback, callBackWhenExceptionOccur);
#endif
        }

        public void ReportClientInfo(ClientInfo info, Action<bool> callback, Action<RemoteServiceException> callBackWhenExceptionOccur)
        {
            AsyncRemoteCall(this.Remote.ReportClientInfo, info, callback, callBackWhenExceptionOccur);
        }

        public void QueryServicePage(string service, Action<string> callback, Action<RemoteServiceException> callBackWhenExceptionOccur)
        {
            AsyncRemoteCall(this.Remote.QueryServicePage, service, callback, callBackWhenExceptionOccur);
        }

        static TankInspectorRemoteService()
        {
            TankInspectorRemoteService.Instance = new TankInspectorRemoteService();
        }
    }
}

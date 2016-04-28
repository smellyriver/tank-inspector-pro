using Smellyriver.TankInspector.Pro.Networking.Livestat;
using Smellyriver.TankInspector.Pro.Networking.Remoting;
using Smellyriver.TankInspector.Pro.Networking.Remoting.XmlRpc;

namespace Smellyriver.TankInspector.Pro.Networking
{
    [XmlRpcUrl(RemoteServiceConfiguration.ServerURL)]
    public interface ITankInspectorRemoteService : IRemoteService
    {
        //no login
        [XmlRpcMethod("tankInspector.register")]
        RegisterResult Register(string email, string password, string invitationCode);

        [XmlRpcMethod("tankInspector.login")]
        bool Login(string user, string password);

        [XmlRpcMethod("tankInspector.login_whit_version")]
        bool LoginEx(string user, string password, int version);

        [XmlRpcMethod("tankInspector.login_pro")]
        bool LoginPro(string user, string password, int version);

        [XmlRpcMethod("tankInspector.logout")]
        bool Logout();

        [XmlRpcMethod("tankInspector.is_service_enabled")]
        bool IsServiceEnabled(string serviceName);

        //need login
        [XmlRpcMethod("tankInspector.check")]
        CheckResult Check();

        [XmlRpcMethod("tankInspector.get_notifications")]
        Notification[] GetNotifications();

        [XmlRpcMethod("tankInspector.dynastats.get_dynastats")]
        LivestatData GetLivestat(int type_comp_descr, int client_version_number, bool fall_back_to_live_version);

        [XmlRpcMethod("tankInspector.resend_activation_email")]
        ResendActivationEmailResult ResendActivationEmail();

        [XmlRpcMethod("tankInspector.send_password_reset_email")]
        bool SendPasswordResetEmail(string email);

        [XmlRpcMethod("tankInspector.get_account_info")]
        AccountInfo GetAccountInfo();

        [XmlRpcMethod("tankInspector.report_client_info")]
        bool ReportClientInfo(ClientInfo info);

        [XmlRpcMethod("tankInspector.query_service_page_url")]
        string QueryServicePage(string service);
    }


}

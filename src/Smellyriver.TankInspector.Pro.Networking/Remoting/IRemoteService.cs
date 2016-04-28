using Smellyriver.TankInspector.Pro.Networking.Remoting.XmlRpc;

namespace Smellyriver.TankInspector.Pro.Networking.Remoting
{
    public partial interface IRemoteService
    {
        [XmlRpcMethod("system.describe")]
        SupportedMethodsDescription Describe();

        [XmlRpcMethod("system.listMethods")]
        string[] ListMethods();

        [XmlRpcMethod("system.methodHelp")]
        string MethodHelp(string method_name);

        [XmlRpcMethod("system.methodSignature")]
        string[] MethodSignature(string method_name);

#if RESTRICT_OOTB_AUTH
        [XmlRpcMethod("system.login")]
        bool Login(string user,string password);

        [XmlRpcMethod("system.logout")]
        bool Logout();
#endif
    }
}

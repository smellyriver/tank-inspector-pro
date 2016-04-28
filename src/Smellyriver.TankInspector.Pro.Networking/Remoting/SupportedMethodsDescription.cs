using Smellyriver.TankInspector.Pro.Networking.Remoting.XmlRpc;

namespace Smellyriver.TankInspector.Pro.Networking.Remoting
{
    public struct SupportedMethodsDescription
    {
        [XmlRpcMember("serviceType")]
        public string ServiceType;

        [XmlRpcMember("serviceURL")]
        public string[] ServiceURL;

        [XmlRpcMember("methods")]
        public MethodDescription[] Methods;
    }
}

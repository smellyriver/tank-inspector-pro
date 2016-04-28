using Smellyriver.TankInspector.Pro.Networking.Remoting.XmlRpc;

namespace Smellyriver.TankInspector.Pro.Networking.Remoting
{
    public struct ParamDescription
    {
        [XmlRpcMember("name")]
        public string Name;
        [XmlRpcMember("rpctype")]
        public string RpcType;
    }
}

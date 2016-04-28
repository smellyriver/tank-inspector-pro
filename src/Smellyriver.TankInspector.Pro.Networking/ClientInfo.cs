using Smellyriver.TankInspector.Pro.Networking.Remoting.XmlRpc;

namespace Smellyriver.TankInspector.Pro.Networking
{
    public struct ClientInfo
    {
        [XmlRpcMember("language")]
        public string Language;
    }
}

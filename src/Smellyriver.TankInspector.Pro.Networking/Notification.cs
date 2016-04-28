using Smellyriver.TankInspector.Pro.Networking.Remoting.XmlRpc;

namespace Smellyriver.TankInspector.Pro.Networking
{
    public struct Notification
    {
        [XmlRpcMember("label")]
        public string Label;

        [XmlRpcMember("display")]
        public string Display;

        [XmlRpcMember("description")]
        public string Description;
    }
}

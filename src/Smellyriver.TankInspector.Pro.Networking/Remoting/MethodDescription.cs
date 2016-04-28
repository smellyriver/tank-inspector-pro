using Smellyriver.TankInspector.Pro.Networking.Remoting.XmlRpc;

namespace Smellyriver.TankInspector.Pro.Networking.Remoting
{
    public struct MethodDescription
    {
        [XmlRpcMember("name")]
        public string Name;

        [XmlRpcMember("summary")]
        public string Summary;

        [XmlRpcMember("return")]
        public string Return;

        [XmlRpcMember("params")]
        public ParamDescription[] Params;

    }
}

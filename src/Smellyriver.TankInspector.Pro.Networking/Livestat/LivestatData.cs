using System.Runtime.Serialization;

namespace Smellyriver.TankInspector.Pro.Networking.Livestat
{
    [DataContract(Name = "Livestat", Namespace = "http://schemas.smellyriver.com/stipro/stats")]
    public class LivestatData
    {
        [DataMember]
        public int type_comp_descr { get; set; }
        [DataMember]
        public int client_version { get; set; }
        [DataMember]
        public VBAddictLivestatData vbaddict { get; set; }
    }
}

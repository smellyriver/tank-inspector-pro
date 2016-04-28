using System;
using System.Runtime.Serialization;
using Smellyriver.TankInspector.Pro.Data.Stats;
using Smellyriver.TankInspector.Pro.Networking.Livestat;

namespace Smellyriver.TankInspector.Pro.LivestatProvider
{
    [DataContract(Name = "LivestatCache", Namespace = Stat.Xmlns)]
    class LivestatItemCache
    {
        [DataMember]
        public ulong Key { get; set; }
        [DataMember]
        public DateTime CachedTime { get; set; }
        [DataMember]
        public LivestatData Livestats { get; set; }
    }
}

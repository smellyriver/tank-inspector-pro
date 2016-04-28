using System.Runtime.Serialization;
using Smellyriver.TankInspector.Pro.Modularity;

namespace Smellyriver.TankInspector.Pro.TechTree
{
    [DataContract]
    class TechTreeDocumentPersistentInfo : DocumentPersistentInfoProviderBase
    {
        [DataMember]
        public string NationKey { get; set; }

        public TechTreeDocumentPersistentInfo(string nationKey)
        {
            this.NationKey = nationKey;
        }
    }
}

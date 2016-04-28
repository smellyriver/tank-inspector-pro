using System.Runtime.Serialization;
using Smellyriver.TankInspector.Pro.Modularity;

namespace Smellyriver.TankInspector.Pro.InternalModules.RepositoryManager
{
    [DataContract]
    class RepositoryManagerDocumentPersistentInfo : DocumentPersistentInfoProviderBase
    {
        [DataMember]
        public string RepositoryID { get; set; }
    }
}

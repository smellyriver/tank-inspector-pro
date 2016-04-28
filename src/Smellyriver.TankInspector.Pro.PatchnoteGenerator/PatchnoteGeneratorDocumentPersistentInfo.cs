using System.Runtime.Serialization;
using Smellyriver.TankInspector.Pro.Modularity;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator
{
    [DataContract]
    class PatchnoteGeneratorDocumentPersistentInfo : DocumentPersistentInfoProviderBase
    {
        [DataMember]
        public string SaveAsPath { get; set; }

        [DataMember]
        public string GeneratedDocument { get; set; }
    }
}

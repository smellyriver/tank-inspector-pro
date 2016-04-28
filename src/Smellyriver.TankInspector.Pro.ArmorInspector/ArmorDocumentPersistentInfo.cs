using System.Runtime.Serialization;
using Smellyriver.TankInspector.Pro.ModelShared;
using Smellyriver.TankInspector.Pro.Modularity;

namespace Smellyriver.TankInspector.Pro.ArmorInspector
{
    [DataContract]
    class ArmorDocumentPersistentInfo : ModelDocumentPersistentInfoBase
    {
        public static ArmorDocumentPersistentInfo Deserialize(string persistentInfo)
        {
            return DocumentPersistentInfoProviderBase.Deserialize<ArmorDocumentPersistentInfo>(persistentInfo);
        }



        public ArmorDocumentPersistentInfo()
        {

        }

    }
}

using System.Runtime.Serialization;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.IO;
using Smellyriver.TankInspector.Pro.ModelShared;
using Smellyriver.TankInspector.Pro.Modularity;

namespace Smellyriver.TankInspector.Pro.ModelInspector
{
    [DataContract]
    class ModelDocumentPersistentInfo : ModelDocumentPersistentInfoBase
    {
        public static ModelDocumentPersistentInfo Deserialize(string persistentInfo)
        {
            return DocumentPersistentInfoProviderBase.Deserialize<ModelDocumentPersistentInfo>(persistentInfo);
        }

        [DataMember]
        public ModelType ModelType { get; set; }

        [DataMember]
        public TextureMode TextureMode { get; set; }

        [DataMember]
        public FileSource TextureSource { get; set; }

        [DataMember]
        public bool ShowCamouflage { get; set; }



        public ModelDocumentPersistentInfo()
        {
            this.ModelType = ModelType.Undamaged;
            this.TextureMode = TextureMode.Normal;
            this.TextureSource = FileSource.Package;
            this.ShowCamouflage = true;
        }

    }
}

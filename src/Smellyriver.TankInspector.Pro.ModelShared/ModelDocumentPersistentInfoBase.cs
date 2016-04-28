using System.Runtime.Serialization;
using Smellyriver.TankInspector.Pro.Graphics;
using Smellyriver.TankInspector.Pro.Graphics.Scene;
using Smellyriver.TankInspector.Pro.Modularity;

namespace Smellyriver.TankInspector.Pro.ModelShared
{
    [DataContract]
    public abstract class ModelDocumentPersistentInfoBase : DocumentPersistentInfoProviderBase
    {
        [DataMember]
        public bool ShowGun { get; set; }
        [DataMember]
        public bool ShowTurret { get; set; }
        [DataMember]
        public bool ShowHull { get; set; }
        [DataMember]
        public bool ShowChassis { get; set; }
        [DataMember]
        public bool UseWireframeMode { get; set; }
        [DataMember]
        public bool ShowFps { get; set; }
        [DataMember]
        public bool ShowTriangleCount { get; set; }
        [DataMember]
        public RotationCenter RotationCenter { get; set; }

        [DataMember]
        public Camera Camera { get; set; }
        [DataMember]
        public string CaptureFilename { get; set; }
        public ModelDocumentPersistentInfoBase()
        {
            this.ShowGun = true;
            this.ShowTurret = true;
            this.ShowHull = true;
            this.ShowChassis = true;
            this.Camera = new Camera();
        }
    }
}

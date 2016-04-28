using System.Runtime.Serialization;

namespace Smellyriver.TankInspector.Pro.Graphics
{
    [DataContract]
    public class GraphicsSettings
    {
        public static readonly GraphicsSettings Default = new GraphicsSettings();

        [DataMember]
        public bool NormalTextureEnabled { get; set; }
        [DataMember]
        public bool SpecularTextureEnabled { get; set; }
        [DataMember]
        public int AnisotropicFilterLevel { get; set; }
        [DataMember]
        public bool CollisionModelStrokeEnabled { get; set; }
        [DataMember]
        public bool SSAOEnabled { get; set; }
        [DataMember]
        public bool SMAAEnabled { get; set; }
        [DataMember]
        public bool WireframeMode { get; set; }

        public GraphicsSettings()
        {
            this.NormalTextureEnabled = true;
            this.SpecularTextureEnabled = true;
            this.AnisotropicFilterLevel = 16;
            this.CollisionModelStrokeEnabled = true;
            this.SSAOEnabled = true;
            this.SMAAEnabled = true;
            this.WireframeMode = false;
        }

    }
}

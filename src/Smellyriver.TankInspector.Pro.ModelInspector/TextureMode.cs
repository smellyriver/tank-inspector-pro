using System.Runtime.Serialization;

namespace Smellyriver.TankInspector.Pro.ModelInspector
{
    [DataContract]
    enum TextureMode
    {
        [EnumMember]
        Normal,
        [EnumMember]
        Grid
    }
}

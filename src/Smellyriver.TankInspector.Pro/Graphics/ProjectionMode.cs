using System.Runtime.Serialization;

namespace Smellyriver.TankInspector.Pro.Graphics
{
    [DataContract]
    public enum ProjectionMode
    {
        [EnumMember]
        Orthographic,
        [EnumMember]
        Perspective
    }
}

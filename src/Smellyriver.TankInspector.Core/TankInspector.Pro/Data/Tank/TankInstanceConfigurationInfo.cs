using System;
using System.Runtime.Serialization;

namespace Smellyriver.TankInspector.Pro.Data.Tank
{
    [DataContract]
    public class TankInstanceConfigurationInfo : ICloneable
    {
        [DataMember]
        internal TankConfigurationInfo TankConfigurationInfo { get; set; }
        [DataMember]
        internal CrewConfigurationInfo CrewConfigurationInfo { get; set; }
        [DataMember]
        internal CustomizationConfigurationInfo CustomizationConfigurationInfo { get; set; }
        [DataMember]
        internal TankTransform Transform { get; set; }

        public TankInstanceConfigurationInfo()
        {
            this.Transform = new TankTransform();
        }

        public TankInstanceConfigurationInfo Clone()
        {
            return new TankInstanceConfigurationInfo
            {
                TankConfigurationInfo = this.TankConfigurationInfo.Clone(),
                CrewConfigurationInfo = this.CrewConfigurationInfo.Clone(),
                CustomizationConfigurationInfo = this.CustomizationConfigurationInfo.Clone(),
                Transform = this.Transform.Clone()
            };
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}

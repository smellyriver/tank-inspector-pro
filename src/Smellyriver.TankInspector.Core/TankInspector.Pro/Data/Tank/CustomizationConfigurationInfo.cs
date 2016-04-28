using System;
using System.Runtime.Serialization;

namespace Smellyriver.TankInspector.Pro.Data.Tank
{
    [DataContract]
    public class CustomizationConfigurationInfo : ICloneable
    {
        public const int NoInscription = -1;
        public const int NoCamouflage = -1;

        [DataMember]
        internal int InscriptionId { get; set; }

        [DataMember]
        internal int CamouflageId { get; set; }

        public CustomizationConfigurationInfo()
        {
            this.InscriptionId = NoInscription;
            this.CamouflageId = NoCamouflage;
        }

        public CustomizationConfigurationInfo Clone()
        {
            return (CustomizationConfigurationInfo)this.MemberwiseClone();
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}

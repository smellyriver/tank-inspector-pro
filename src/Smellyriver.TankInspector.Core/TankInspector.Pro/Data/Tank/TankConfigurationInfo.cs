using System;
using System.Runtime.Serialization;

namespace Smellyriver.TankInspector.Pro.Data.Tank
{
    [DataContract]
    public class TankConfigurationInfo : ICloneable
    {
        [DataMember]
        internal string GunKey { get; set; }
        [DataMember]
        internal string TurretKey { get; set; }
        [DataMember]
        internal string ChassisKey { get; set; }
        [DataMember]
        internal string RadioKey { get; set; }
        [DataMember]
        internal string EngineKey { get; set; }
        [DataMember]
        internal string AmmunitionKey { get; set; }
        [DataMember]
        internal string[] EquipmentKeys { get; private set; }
        [DataMember]
        internal string[] ConsumableKeys { get; private set; }

        internal TankConfigurationInfo()
        {
            this.EquipmentKeys = new string[3];
            this.ConsumableKeys = new string[3];
        }


        public TankConfigurationInfo Clone()
        {
            return (TankConfigurationInfo)this.MemberwiseClone();
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}

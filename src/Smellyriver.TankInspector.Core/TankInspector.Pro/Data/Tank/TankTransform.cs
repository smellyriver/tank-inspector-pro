using System;
using System.Runtime.Serialization;

namespace Smellyriver.TankInspector.Pro.Data.Tank
{
    [DataContract]
    public class TankTransform : ICloneable
    {
        [DataMember(Name="TurretYaw")]
        private double _turretYaw;

        public double TurretYaw
        {
            get { return _turretYaw; }
            set
            {
                if (_turretYaw == value)
                    return;

                _turretYaw = value;
                if (this.TurretYawChanged != null)
                    this.TurretYawChanged(this, EventArgs.Empty);
            }
        }

        [DataMember(Name = "GunPitch")]
        private double _gunPitch;

        public double GunPitch
        {
            get { return _gunPitch; }
            set
            {
                if (_gunPitch == value)
                    return;

                _gunPitch = value;
                if (this.GunPitchChanged != null)
                    this.GunPitchChanged(this, EventArgs.Empty);
            }
        }


        [DataMember(Name = "VehicleYaw")]
        private double _vehicleYaw;

        public double VehicleYaw
        {
            get { return _vehicleYaw; }
            set
            {
                if (_vehicleYaw == value)
                    return;

                _vehicleYaw = value;
                if (this.VehicleYawChanged != null)
                    this.VehicleYawChanged(this, EventArgs.Empty);
            }
        }
        
        public event EventHandler TurretYawChanged;
        public event EventHandler GunPitchChanged;
        public event EventHandler VehicleYawChanged;

        internal TankTransform Clone()
        {
            return (TankTransform)this.MemberwiseClone();
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}

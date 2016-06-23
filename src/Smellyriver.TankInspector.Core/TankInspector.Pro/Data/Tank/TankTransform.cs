using System;
using System.Runtime.Serialization;

namespace Smellyriver.TankInspector.Pro.Data.Tank
{
    [DataContract]
    public class TankTransform : ICloneable
    {
        [DataMember(Name = "TurretYaw")]
        private double _turretYaw;

        public double TurretYaw
        {
            get { return _turretYaw; }
            set
            {
                if (_turretYaw == value)
                    return;

                _turretYaw = value;
                if (_turretYawChanged != null)
                    _turretYawChanged(this, EventArgs.Empty);
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
                if (_gunPitchChanged != null)
                    _gunPitchChanged(this, EventArgs.Empty);
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
                if (_vehicleYawChanged != null)
                    _vehicleYawChanged(this, EventArgs.Empty);
            }
        }

        private EventHandler _turretYawChanged;
        public event EventHandler TurretYawChanged { add { _turretYawChanged += value; } remove { _turretYawChanged -= value; } }

        private EventHandler _gunPitchChanged;
        public event EventHandler GunPitchChanged { add { _gunPitchChanged += value; } remove { _gunPitchChanged -= value; } }

        private EventHandler _vehicleYawChanged;
        public event EventHandler VehicleYawChanged { add { _vehicleYawChanged += value; } remove { _vehicleYawChanged -= value; } }


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

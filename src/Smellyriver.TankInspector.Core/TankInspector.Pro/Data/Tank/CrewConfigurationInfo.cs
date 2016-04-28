using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Smellyriver.TankInspector.Pro.Gameplay;
using Smellyriver.Utilities;

namespace Smellyriver.TankInspector.Pro.Data.Tank
{
    [DataContract]
    public class CrewConfigurationInfo : ICloneable
    {
        [DataMember(Name = "Crews")]
        private Dictionary<string, CrewInstanceInfo> _crews;

        public CrewConfigurationInfo()
        {
            _crews = new Dictionary<string, CrewInstanceInfo>();
        }

        internal CrewInstanceInfo this[string role]
        {
            get
            {
                CrewHelper.ThrowIfRoleInvalid(role);
                return _crews.GetOrCreate(role, () => new CrewInstanceInfo(role));
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                CrewHelper.ThrowIfRoleInvalid(role);

                _crews[role] = value;
            }
        }

        public CrewConfigurationInfo Clone()
        {
            var clone = new CrewConfigurationInfo();
            foreach(var kvp in _crews)
            {
                clone._crews.Add(kvp.Key, kvp.Value.Clone());
            }

            return clone;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}

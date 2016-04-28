using System;
using System.Runtime.Serialization;
using Smellyriver.TankInspector.Pro.Gameplay;

namespace Smellyriver.TankInspector.Pro.Data.Tank
{
    [DataContract]
    public class CrewInstanceInfo : ICloneable
    {
        [DataMember]
        internal bool IsDead { get; set; }

        [DataMember]
        internal int LastSkillTrainingLevel { get; set; }

        [DataMember]
        internal string[] SkillKeys { get; set; }

        [DataMember]
        internal string Role { get; private set; }

        public CrewInstanceInfo(string role)
        {
            CrewHelper.ThrowIfRoleInvalid(role);
            this.Role = role;
            this.SkillKeys = new string[0];
        }

        public CrewInstanceInfo Clone()
        {
            return (CrewInstanceInfo)this.MemberwiseClone();
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}

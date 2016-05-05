using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Smellyriver.TankInspector.Pro.Data.Tank.Scripting
{
    class SkillScript : SkillScriptBase
    {
        public static SkillScript Create(IXQueryable skill, Func<double> levelGetter)
        {
            switch (skill["@key"])
            {
                case "repair":
                case "fireFighting":
                    return new SkillScript(skill, SkillType.Skill, skill["@role"], levelGetter, DuplicatedSkillPolicy.Average, 0);
                case "camouflage":
                    return new CamouflageSkillScript(skill, levelGetter);
                case "brotherhood":
                    return new SkillScript(skill, SkillType.Perk, skill["@role"], levelGetter, DuplicatedSkillPolicy.Lowest, -100);
                case "tutor":
                case "smoothDriving":
                case "virtuoso":
                case "badRoadsKing":
                case "rammingMaster":
                case "smoothTurret":
                case "gunsmith":
                case "finder":
                case "inventor":
                case "retransmitter":
                case "eagleEye":
                    return new SkillScript(skill, SkillType.Skill, skill["@role"], levelGetter, DuplicatedSkillPolicy.Highest, 0);
                case "tidyPerson":
                case "expert":
                case "universalist":
                case "sixthSense":
                case "sniper":
                case "rancorous":
                case "woodHunter":
                case "pedant":
                case "desperado":
                case "intuition":
                case "lastEffort":
                    return new SkillScript(skill, SkillType.Perk, skill["@role"], levelGetter, DuplicatedSkillPolicy.Highest, 0);
                default:
                    throw new NotSupportedException();
            }
        }

        private readonly DuplicatedSkillPolicy _duplicatedSkillPolicy;
        protected override DuplicatedSkillPolicy DuplicatedSkillPolicy
        {
            get { return _duplicatedSkillPolicy; }
        }

        private readonly SkillType _skillType;
        protected override SkillType SkillType
        {
            get { return _skillType; }
        }


        private readonly int _priority;
        public override int Priority
        {
            get { return _priority; }
        }

        private readonly string _domain;
        protected override string Domain
        {
            get { return _domain; }
        }

        private readonly IXQueryable _skill;

        public IXQueryable Skill
        {
            get { return _skill; }
        }

        private readonly Dictionary<string, double> _parameters;

        protected Dictionary<string, double> Parameters
        {
            get { return _parameters; }
        }

        private readonly string _role;
        protected override string Role
        {
            get { return _role; }
        }

        public SkillScript(IXQueryable skill, SkillType skillType, string role, Func<double> levelGetter, DuplicatedSkillPolicy duplicatedSkillPolicy, int priority)
            : base(levelGetter)
        {
            this._skill = skill;
            _role = role;

            var skillKey = this.Skill["@key"];
            _domain = skillKey.Substring(0, 1).ToLower() + skillKey.Substring(1);

            double dummy;
            this._parameters = this.Skill.Where(i => i.Name != "userString"
                                               && i.Name != "description"
                                               && i.Name != "icon"
                                               && double.TryParse(i.Value, 
                                                                  NumberStyles.Float | NumberStyles.AllowThousands, 
                                                                  CultureInfo.InvariantCulture,  
                                                                  out dummy))
                                        .ToDictionary(i => i.Name, i => double.Parse(i.Value, CultureInfo.InvariantCulture));

            _duplicatedSkillPolicy = duplicatedSkillPolicy;
            _skillType = skillType;
            _priority = priority;
        }


        protected override void Execute(ScriptingContext context, double level)
        {
            if (this.SkillType == SkillType.Perk)
            {
                foreach (var parameter in this.Parameters)
                    context.SetValue(this.Domain, parameter.Key, parameter.Value);
            }
            else
            {
                foreach (var parameter in this.Parameters)
                {
                    var key = parameter.Key;
                    key = key.Replace("FactorPerLevel", "Factor");
                    context.SetValue(this.Domain, key, parameter.Value * level);
                }
            }
        }

        protected override void Clear(ScriptingContext context)
        {
            foreach (var parameter in this.Parameters)
                context.SetValue(this.Domain, parameter.Key, 0.0);
        }
    }
}

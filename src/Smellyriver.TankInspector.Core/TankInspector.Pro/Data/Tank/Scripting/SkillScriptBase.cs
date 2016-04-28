using System;
using System.Collections.Generic;
using System.Linq;

namespace Smellyriver.TankInspector.Pro.Data.Tank.Scripting
{
    abstract class SkillScriptBase : Script
    {
        protected abstract DuplicatedSkillPolicy DuplicatedSkillPolicy { get; }
        protected abstract SkillType SkillType { get; }
        protected abstract string Domain { get; }
        protected abstract string Role { get; }

        public override int Priority
        {
            get { return 0; }
        }
        public Func<double> LevelGetter { get; set; }

        public SkillScriptBase(Func<double> levelGetter)
        {
            this.LevelGetter = levelGetter;
        }

        public override void Execute(ScriptingContext context)
        {
            var currentLevel = this.LevelGetter();
            var levels = new List<double>();
            levels.Add(currentLevel);

            var i = 0;

            for (; ; ++i)
            {
                var level = context.GetValue(this.Domain, string.Format("__level{0}", i));
                if (level == null)
                    break;
                else
                    levels.Add(level.Value);
            }

            context.SetValue(this.Domain, string.Format("__level{0}", i), currentLevel);

            var crewCount = context.GetCrewCount(this.Role);

            for (++i; i < crewCount; ++i)
            {
                levels.Add(0);
            }

            double actualValue;

            switch (this.DuplicatedSkillPolicy)
            {
                case DuplicatedSkillPolicy.Average:
                    actualValue = levels.Average();
                    break;
                case DuplicatedSkillPolicy.Highest:
                    actualValue = levels.Max();
                    break;
                case DuplicatedSkillPolicy.Lowest:
                    actualValue = levels.Min();
                    break;
                default:
                    throw new NotSupportedException();
            }

            if (this.SkillType == SkillType.Skill || (this.SkillType == SkillType.Perk && actualValue >= 100))
                this.Execute(context, actualValue);
            else
                this.Clear(context);
        }


        protected abstract void Execute(ScriptingContext context, double level);

        protected abstract void Clear(ScriptingContext context);
    }
}

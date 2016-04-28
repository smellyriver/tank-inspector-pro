using System;

namespace Smellyriver.TankInspector.Pro.Data.Tank.Scripting
{
    sealed class CommanderBasicSkillScript : BasicSkillScript
    {
        public const string ViewRangeFactorSkillKey = "viewRangeFactor";
        public const string CrewTrainingLevelBuffSkillKey = "crewTrainingLevelBuff";

        protected override string Role
        {
            get { return "commander"; }
        }

        public override int Priority
        {
            get { return -10; }
        }

        public CommanderBasicSkillScript(Func<double> levelGetter)
            : base(levelGetter)
        {

        }

        protected override void Execute(ScriptingContext context, double level)
        {
            var viewRangeFactor = BasicSkillScript.GetIncrementalSkillFactor(level);
            context.SetValue(this.Domain, ViewRangeFactorSkillKey, viewRangeFactor);
            var crewTrainingLevelBuff = level / 10;
            context.SetValue(this.Domain, CrewTrainingLevelBuffSkillKey, crewTrainingLevelBuff);
        }
    }
}

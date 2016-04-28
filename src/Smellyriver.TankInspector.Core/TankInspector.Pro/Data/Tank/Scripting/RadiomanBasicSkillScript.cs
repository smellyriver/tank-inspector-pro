using System;

namespace Smellyriver.TankInspector.Pro.Data.Tank.Scripting
{
    sealed class RadiomanBasicSkillScript : BasicSkillScript
    {
        public const string SignalRangeFactorSkillKey = "signalRangeFactor";

        protected override string Role
        {
            get { return "radioman"; }
        }

        public RadiomanBasicSkillScript(Func<double> levelGetter)
            : base(levelGetter)
        {

        }

        protected override void Execute(ScriptingContext context, double level)
        {
            var signalRangeFactor = BasicSkillScript.GetIncrementalSkillFactor(level);
            context.SetValue(this.Domain, SignalRangeFactorSkillKey, signalRangeFactor);
        }
    }
}

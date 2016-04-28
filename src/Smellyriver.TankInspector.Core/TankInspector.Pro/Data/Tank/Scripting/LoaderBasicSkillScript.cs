using System;

namespace Smellyriver.TankInspector.Pro.Data.Tank.Scripting
{
    sealed class LoaderBasicSkillScript : BasicSkillScript
    {
        public const string LoadTimeFactorSkillKey = "loadTimeFactor";

        protected override string Role
        {
            get { return "loader"; }
        }

        public LoaderBasicSkillScript(Func<double> levelGetter)
            : base(levelGetter)
        {

        }

        protected override void Execute(ScriptingContext context, double level)
        {
            var loadTimeFactor = BasicSkillScript.GetDecrementalSkillFactor(level);
            context.SetValue(this.Domain, LoadTimeFactorSkillKey, loadTimeFactor);
        }
    }
}

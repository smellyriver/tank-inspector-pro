using System;

namespace Smellyriver.TankInspector.Pro.Data.Tank.Scripting
{
    sealed class CamouflageSkillScript : SkillScript
    {
        public const string CamouflageFactorSkillKey = "camouflageFactor";

        public CamouflageSkillScript(IXQueryable skill, Func<double> levelGetter)
            : base(skill, SkillType.Skill, null, levelGetter, DuplicatedSkillPolicy.Average, 0)
        {

        }

        protected override void Execute(ScriptingContext context, double level)
        {
            context.SetValue(this.Domain, CamouflageFactorSkillKey, 1.0 + 0.0075 * level);
        }

        protected override void Clear(ScriptingContext context)
        {
            context.SetValue(this.Domain, CamouflageFactorSkillKey, 1.0);
        }
    }
}

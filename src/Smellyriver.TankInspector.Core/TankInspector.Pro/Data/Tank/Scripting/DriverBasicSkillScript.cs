using System;

namespace Smellyriver.TankInspector.Pro.Data.Tank.Scripting
{

    sealed class DriverBasicSkillScript : BasicSkillScript
    {
        public const string TerrainResistanceFactorSkillKey = "terrainResistanceFactor";

        protected override string Role
        {
            get { return "driver"; }
        }

        public DriverBasicSkillScript(Func<double> levelGetter)
            : base(levelGetter)
        {

        }

        protected override void Execute(ScriptingContext context, double level)
        {
            var terrainResistanceFactor = BasicSkillScript.GetDecrementalSkillFactor(level);
            context.SetValue(this.Domain, TerrainResistanceFactorSkillKey, terrainResistanceFactor);
        }
    }
}

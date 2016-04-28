using System;

namespace Smellyriver.TankInspector.Pro.Data.Tank.Scripting
{

    sealed class GunnerBasicSkillScript : BasicSkillScript
    {
        public const string AccuracyFactorSkillKey = "accuracyFactor";
        public const string AimingTimeFactorSkillKey = "aimingTimeFactor";
        public const string ShotDispersionFactorSkillKey = "shotDispersionFactor";
        public const string TurretRotationSpeedSkillKey = "turretRotationSpeed";

        protected override string Role
        {
            get { return "gunner"; }
        }

        public GunnerBasicSkillScript(Func<double> levelGetter)
            : base(levelGetter)
        {

        }

        protected override void Execute(ScriptingContext context, double level)
        {
            var decrementalFactor = BasicSkillScript.GetDecrementalSkillFactor(level);
            context.SetValue(this.Domain, AccuracyFactorSkillKey, decrementalFactor);
            context.SetValue(this.Domain, AimingTimeFactorSkillKey, decrementalFactor);
            context.SetValue(this.Domain, ShotDispersionFactorSkillKey, decrementalFactor);

            var incrementalFactor = BasicSkillScript.GetIncrementalSkillFactor(level);
            context.SetValue(this.Domain, TurretRotationSpeedSkillKey, incrementalFactor);
        }
    }
}

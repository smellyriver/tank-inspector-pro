using System;

namespace Smellyriver.TankInspector.Pro.Data.Tank.Scripting
{
    abstract class BasicSkillScript : SkillScriptBase
    {

        public static BasicSkillScript Create(string role, Func<double> levelGetter)
        {
            switch (role)
            {
                case "commander":
                    return new CommanderBasicSkillScript(levelGetter);
                case "driver":
                    return new DriverBasicSkillScript(levelGetter);
                case "gunner":
                    return new GunnerBasicSkillScript(levelGetter);
                case "radioman":
                    return new RadiomanBasicSkillScript(levelGetter);
                case "loader":
                    return new LoaderBasicSkillScript(levelGetter);
                default:
                    throw new NotSupportedException();
            }
        }

        internal static double GetIncrementalSkillFactor(double level)
        {
            return (0.00375 * level + 0.5) / 0.875;
        }

        internal static double GetDecrementalSkillFactor(double level)
        {
            return 0.875 / (0.00375 * level + 0.5);
        }

        protected override DuplicatedSkillPolicy DuplicatedSkillPolicy
        {
            get { return DuplicatedSkillPolicy.Average; }
        }

        protected override SkillType SkillType
        {
            get { return SkillType.Skill; }
        }

        protected override string Domain
        {
            get { return this.Role; }
        }

        public BasicSkillScript(Func<double> levelGetter)
            : base(levelGetter)
        {

        }


        protected override void Clear(ScriptingContext context)
        {

        }

    }
}

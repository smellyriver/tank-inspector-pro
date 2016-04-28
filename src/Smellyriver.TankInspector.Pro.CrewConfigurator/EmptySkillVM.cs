namespace Smellyriver.TankInspector.Pro.CrewConfigurator
{
    class EmptySkillVM : SkillVMBase
    {
        public static readonly EmptySkillVM Instance = new EmptySkillVM();

        private EmptySkillVM()
            : base(null)
        {

        }
    }
}

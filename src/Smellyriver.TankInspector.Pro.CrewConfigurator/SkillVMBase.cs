using Smellyriver.TankInspector.Pro.Data;

namespace Smellyriver.TankInspector.Pro.CrewConfigurator
{
    abstract class SkillVMBase
    {
        public IXQueryable Model { get; private set; }
        public SkillVMBase(IXQueryable skill)
        {
            this.Model = skill;
        }

    }
}

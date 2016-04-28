using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smellyriver.TankInspector.Pro.TankModuleTree
{
    class UnlockInfoVM
    {
        public IModuleUnlockTargetVM Target { get; private set; }
        public double Experience { get; private set; }

        public UnlockInfoVM(IModuleUnlockTargetVM target, double experience)
        {
            this.Target = target;
            this.Experience = experience;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smellyriver.TankInspector.Pro.TankModuleTree
{
    interface IModuleUnlockTargetVM
    {
        int Row { get; set; }
        int Column { get; set; }
    }
}

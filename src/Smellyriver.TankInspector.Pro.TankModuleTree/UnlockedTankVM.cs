using Smellyriver.TankInspector.Pro.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smellyriver.TankInspector.Pro.TankModuleTree
{
    class UnlockedTankVM : IModuleUnlockTargetVM
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public ModuleVM UnlockedBy { get; private set; }

        public Tank Tank { get; private set; }
        public UnlockedTankVM(Tank tank, ModuleVM unlockedBy)
        {
            this.UnlockedBy = unlockedBy;
            this.Row = ModuleUnlockTargetVM.UndefinedRowOrColumn;
            this.Column = ModuleUnlockTargetVM.UndefinedRowOrColumn;
        }
    }
}

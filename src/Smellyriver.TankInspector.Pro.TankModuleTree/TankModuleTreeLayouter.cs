using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smellyriver.TankInspector.Pro.TankModuleTree
{
    class TankModuleTreeLayouter
    {
        private List<RowInfo> _rows;
        private int _rowPointer;
        private IEnumerable<ModuleVM> _modules;

        private void Reset(IEnumerable<ModuleVM> modules)
        {
            _rows = new List<RowInfo>();
            _rowPointer = -1;
        }

        public void Layout(IEnumerable<ModuleVM> modules)
        {
            this.Reset(modules);
            this.LayoutModules();
        }

        private void LayoutModules()
        {
            foreach (var rootModule in _modules.Where(m => m.UnlockExperience == 0))
            {
                var row = this.NewRow();
                this.LayoutModuleRecursive(rootModule, row);
            }
        }

        private void LayoutModuleRecursive(ModuleVM module, RowInfo row)
        {
            row.AppendCell(module);

            var targets = module.Unlocks.Select(u => u.Target).ToArray();
            for (var i = 0; i < targets.Length; ++i)
            {
                var unlockedTarget = targets[i];

                if (i > 0)
                {
                    var newRow = this.NewRow();
                }

                if (unlockedTarget is UnlockedTankVM)
                    row.SetUnlockedTank((UnlockedTankVM)unlockedTarget);
                else
                {

                }
            }
        }

        private RowInfo NewRow()
        {
            ++_rowPointer;
            var row = new RowInfo(_rowPointer);
            _rows.Add(row);

            return row;
        }
    }
}

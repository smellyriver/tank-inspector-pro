using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smellyriver.TankInspector.Pro.TankModuleTree
{
    class RowInfo
    {
        public int Index { get; private set; }

        private readonly Dictionary<int, IModuleUnlockTargetVM> _cells;

        public bool IsTerminated
        {
            get { return this.IsTerminated; }
        }

        public int ColumnIndex { get; private set; }

        public UnlockedTankVM UnlockedTank { get; private set; }

        public RowInfo(int index)
        {
            this.Index = index;
        }

        public IModuleUnlockTargetVM GetCell(int column)
        {
            IModuleUnlockTargetVM target;
            if (_cells.TryGetValue(column, out target))
                return target;

            return null;
        }

        public void SetCell(int column, IModuleUnlockTargetVM target)
        {
            this.CheckTerminated();

            if (column <= this.ColumnIndex)
                throw new ArgumentOutOfRangeException("column");

            this.ColumnIndex = column;

            if (target.Row == ModuleUnlockTargetVM.UndefinedRowOrColumn)
                target.Row = this.Index;

            if (target.Column == ModuleUnlockTargetVM.UndefinedRowOrColumn)
                target.Column = column;

            if (target.Row != this.Index)
                throw new ArgumentException("target");

            if ( target.Column != column)
                return;

            _cells[column] = target;
        }

        public void AppendCell(IModuleUnlockTargetVM target)
        {
            this.SetCell(this.ColumnIndex + 1, target);
        }

        public void SetUnlockedTank(UnlockedTankVM tank)
        {
            this.CheckTerminated();

            if (tank.Row == ModuleUnlockTargetVM.UndefinedRowOrColumn)
                tank.Row = this.Index;

            if (tank.Row != this.Index)
                throw new ArgumentException("tank");

            this.UnlockedTank = tank;
        }

        private void CheckTerminated()
        {
            if (this.IsTerminated)
                throw new InvalidOperationException("this row is terminated");
        }
    }
}


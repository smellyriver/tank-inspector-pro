using System.Collections.Generic;
using System.Linq;
using Smellyriver.TankInspector.Pro.Data;

namespace Smellyriver.TankInspector.Pro.TechTree
{
    class TechTreeLayoutNode
    {

        private string _tankKey;
        public string TankKey { get { return _tankKey; } }

        private int _row;
        public int Row { get { return _row; } }

        private int _column;
        public int Column { get { return _column; } }

        private string[] _unlockTanks;
        public IEnumerable<string> UnlockTanks { get { return _unlockTanks; } }

        public IXQueryable Tank { get; }

        public string Name { get { return this.Tank["userString"]; } }
        public TechTreeLayoutNode(IXQueryable tank)
            : this(tank, tank.QueryInt("row"), tank.QueryInt("column"), tank.QueryManyValues("lines/line/@target"))
        {

        }

        public TechTreeLayoutNode(IXQueryable tank, int row, int column, IEnumerable<string> unlockTanks)
            : this(tank, unlockTanks)
        {
            _row = row;
            _column = column;
        }
        private TechTreeLayoutNode(IXQueryable tank, IEnumerable<string> unlockTanks)
        {
            this.Tank = tank;
            _tankKey = tank["@key"];
            if (unlockTanks == null)
                _unlockTanks = new string[0];
            else
                _unlockTanks = unlockTanks.ToArray();
        }
    }
}

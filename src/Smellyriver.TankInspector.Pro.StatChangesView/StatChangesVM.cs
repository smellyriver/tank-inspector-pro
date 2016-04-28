using System.Collections.Generic;
using System.Linq;
using Smellyriver.TankInspector.Pro.Data.Stats;
using Smellyriver.TankInspector.Pro.Data.Tank;

namespace Smellyriver.TankInspector.Pro.StatChangesView
{
    class StatChangesVM
    {

        private readonly List<StatChangeVM> _statChanges;

        public IEnumerable<StatChangeVM> StatChanges { get { return _statChanges; } }

        public StatChangesVM(TankInstance before, TankInstance after)
        {
            _statChanges = new List<StatChangeVM>();

            foreach (var stat in StatsProviderManager.Instance.Stats.Where(s => s.IsStatic))
            {
                var originalValue = stat.GetValue(before, before.Repository, false);
                var changedValue = stat.GetValue(after, after.Repository, false);

                if (stat.Comparer.Compare(originalValue, changedValue) != 0)
                {
                    _statChanges.Add(new StatChangeVM(stat, originalValue, changedValue));
                }
            }
        }
    }
}

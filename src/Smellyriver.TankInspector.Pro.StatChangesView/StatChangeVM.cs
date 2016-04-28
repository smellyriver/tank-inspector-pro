using System.Windows.Media;
using Smellyriver.TankInspector.Pro.Data.Stats;
using Smellyriver.TankInspector.Pro.StatsShared;

namespace Smellyriver.TankInspector.Pro.StatChangesView
{
    class StatChangeVM
    {

        public string Name { get; private set; }
        public string OriginalValue { get; private set; }
        public string ChangedValue { get; private set; }
        public ImageSource Icon { get; private set; }

        public StatChangeVM(IStat stat, string originalValue, string changedValue)
        {
            this.Name = stat.Name;
            this.OriginalValue = stat.FormatValue(originalValue);
            this.ChangedValue = stat.FormatValue(changedValue);
            this.Icon = ComparisonIcon.GetIcon(stat, originalValue, changedValue);
        }
    }
}

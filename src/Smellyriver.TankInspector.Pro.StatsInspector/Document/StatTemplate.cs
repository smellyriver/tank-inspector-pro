using System.Windows.Documents;
using Smellyriver.TankInspector.Pro.Globalization.Wpf;

namespace Smellyriver.TankInspector.Pro.StatsInspector.Document
{
    public sealed class StatTemplate : FlowDocument
    {
        public StatTemplate()
        {
            Loc.SetAssembly(this, typeof(StatTemplate).Assembly);
        }
    }
}

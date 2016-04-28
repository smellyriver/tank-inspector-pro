using Smellyriver.TankInspector.Pro.Data;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report
{
    abstract class TankDataFieldHandler
    {
        public abstract PatchnoteReportItem[] CreateReportItems(string name, TankDataFieldBase field, IXQueryable oldItem, IXQueryable newItem);
    }
}

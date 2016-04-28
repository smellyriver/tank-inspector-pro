using System.Windows.Documents;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report
{
    abstract class PatchnoteReportItem
    {
        public abstract Block[] CreateBlocks();
    }
}

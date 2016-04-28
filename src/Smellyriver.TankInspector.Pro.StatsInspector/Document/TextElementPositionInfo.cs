using System.Windows.Documents;
using Smellyriver.TankInspector.Common;

namespace Smellyriver.TankInspector.Pro.StatsInspector.Document
{
    class TextElementPositionInfo
    {
        public WeakReference<TextElement> Parent;
        public WeakReference<TextElement>[] PreceedingSiblingsSnapshot;
        public WeakReference<TextElement>[] SucceedingSiblingsSnapshot;
    }
}

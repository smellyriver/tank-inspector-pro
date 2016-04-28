using System.Windows;
using Smellyriver.TankInspector.Common.Wpf.Behaviors.DragDrop;
using Smellyriver.TankInspector.Pro.GameClientExplorer.Vehicles;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer
{
    class ExplorerTreeDragHandlerImpl : IDragSource
    {
        public void StartDrag(DragInfo dragInfo)
        {
            if(dragInfo.SourceItem is TankNodeVM)
            {
                dragInfo.Effects = DragDropEffects.Copy;
                dragInfo.Data = ((TankNodeVM)dragInfo.SourceItem).TankUnikey;
            }
        }
    }
}

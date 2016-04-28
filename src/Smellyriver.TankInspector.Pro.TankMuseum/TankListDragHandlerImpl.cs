using System.Linq;
using System.Windows;
using Smellyriver.TankInspector.Common.Wpf.Behaviors.DragDrop;

namespace Smellyriver.TankInspector.Pro.TankMuseum
{
    class TankListDragHandlerImpl : IDragSource
    {
        public void StartDrag(DragInfo dragInfo)
        {
            var tankVms = dragInfo.SourceItems.OfType<TankVM>().ToArray();
            if (tankVms.Length>0)
            {
                dragInfo.Effects = DragDropEffects.Copy;
                dragInfo.Data = tankVms.Select(t => t.TankUnikey).ToArray();
                return;
            }

            if (dragInfo.SourceItem is TankVM)
            {
                dragInfo.Effects = DragDropEffects.Copy;
                dragInfo.Data = ((TankVM)dragInfo.SourceItem).TankUnikey;
            }
        }
    }
}

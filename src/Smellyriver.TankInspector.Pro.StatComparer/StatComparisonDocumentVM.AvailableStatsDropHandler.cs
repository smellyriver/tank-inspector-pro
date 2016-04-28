using System.Linq;
using System.Windows;
using Smellyriver.TankInspector.Common.Wpf.Behaviors.DragDrop;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    partial class StatComparisonDocumentVM
    {
        private class AvailableStatsDropHandlerImpl : IDropTarget
        {
            private readonly StatsManagerVM _owner;

            public AvailableStatsDropHandlerImpl(StatsManagerVM owner)
            {
                _owner = owner;
            }

            public void DragOver(DropInfo dropInfo)
            {
                if (dropInfo.DragInfo.SourceCollection == _owner.SelectedStats)
                    dropInfo.Effects = DragDropEffects.Move;
                else
                    dropInfo.Effects = DragDropEffects.None;
            }

            public void Drop(DropInfo dropInfo)
            {
                if (dropInfo.DragInfo.SourceCollection == _owner.SelectedStats)
                {
                    var stats = dropInfo.GetEnumerableData<StatInfoVM>().ToList();
                    if (_owner.CanRemoveStat(stats))
                        _owner.RemoveStat(stats);
                }
            }
        }
    }
}

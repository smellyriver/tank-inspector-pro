using System.Windows;
using Smellyriver.TankInspector.Common.Wpf.Behaviors.DragDrop;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    partial class StatComparisonDocumentVM
    {
        private class SelectedStatsDropHandlerImpl : IDropTarget
        {
            private readonly StatsManagerVM _owner;

            public SelectedStatsDropHandlerImpl(StatsManagerVM owner)
            {
                _owner = owner;
            }

            public void DragOver(DropInfo dropInfo)
            {
                if (dropInfo.DragInfo.SourceCollection == _owner.SelectedStats)
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                    dropInfo.Effects = DragDropEffects.Move;
                }
                else if (_owner.CanAddStat(dropInfo.Data))
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                    dropInfo.Effects = DragDropEffects.Move;
                }
                else
                    dropInfo.Effects = DragDropEffects.None;
            }

            public void Drop(DropInfo dropInfo)
            {
                if (dropInfo.DragInfo.SourceCollection == _owner.SelectedStats)
                {
                    var insertIndex = dropInfo.InsertIndex;

                    foreach (var stat in dropInfo.GetEnumerableData<StatInfoVM>())
                    {
                        this.MoveStatItem(stat, insertIndex);
                        ++insertIndex;
                    }
                }
                else if (_owner.CanAddStat(dropInfo.Data))
                {
                    _owner.AddStat(dropInfo.Data, dropInfo.InsertIndex);
                }
            }

            private void MoveStatItem(StatInfoVM stat, int insertIndex)
            {
                var oldIndex = _owner.SelectedStats.IndexOf(stat);
                insertIndex = insertIndex > oldIndex ? insertIndex - 1 : insertIndex;
                _owner.SelectedStats.Move(oldIndex, insertIndex);
            }
        }
    }
}

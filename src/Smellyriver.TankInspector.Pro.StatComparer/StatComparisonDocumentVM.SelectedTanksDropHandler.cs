using System.Linq;
using System.Windows;
using Smellyriver.TankInspector.Common.Wpf.Behaviors.DragDrop;
using Smellyriver.TankInspector.Pro.Data.Tank;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    partial class StatComparisonDocumentVM
    {
        private class SelectedTanksDropHandlerImpl : IDropTarget
        {
            private readonly TanksManagerVM _owner;

            public SelectedTanksDropHandlerImpl(TanksManagerVM owner)
            {
                _owner = owner;
            }

            public void DragOver(DropInfo dropInfo)
            {
                if (dropInfo.DragInfo.SourceCollection == _owner.SelectedTanks)
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                    dropInfo.Effects = DragDropEffects.Move;
                }
                else if (dropInfo.GetEnumerableData<TankUnikey>().Count() > 0)
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                    dropInfo.Effects = DragDropEffects.Copy;
                }
                else
                {
                    dropInfo.Effects = DragDropEffects.None;
                }

            }

            public void Drop(DropInfo dropInfo)
            {
                if (dropInfo.DragInfo.SourceCollection == _owner.SelectedTanks)
                {
                    var insertIndex = dropInfo.InsertIndex;

                    foreach (var tank in dropInfo.GetEnumerableData<TankVM>())
                    {
                        insertIndex = this.MoveTankItem(tank, insertIndex) + 1;
                    }
                }
                else
                {
                    var insertIndex = dropInfo.InsertIndex;
                    foreach (var unikey in dropInfo.GetEnumerableData<TankUnikey>())
                    {
                        var tank = _owner.GetTankVM(unikey);
                        if (tank != null && !_owner.SelectedTanks.Contains(tank))
                        {
                            _owner.SelectedTanks.Insert(insertIndex, tank);
                            ++insertIndex;
                        }
                    }
                }
            }

            // returns actual insert index
            private int MoveTankItem(TankVM tank, int insertIndex)
            {
                var oldIndex = _owner.SelectedTanks.IndexOf(tank);
                insertIndex = insertIndex > oldIndex ? insertIndex - 1 : insertIndex;
                _owner.SelectedTanks.Move(oldIndex, insertIndex);

                return insertIndex;
            }
        }
    }
}

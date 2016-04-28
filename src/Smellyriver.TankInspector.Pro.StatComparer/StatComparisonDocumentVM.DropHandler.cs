using System.Linq;
using System.Windows;
using Smellyriver.TankInspector.Common.Wpf.Behaviors.DragDrop;
using Smellyriver.TankInspector.Pro.Data.Tank;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    partial class StatComparisonDocumentVM
    {
        private class DropHandlerImpl : IDropTarget
        {
            private readonly StatComparisonDocumentVM _owner;

            public DropHandlerImpl(StatComparisonDocumentVM owner)
            {
                _owner = owner;
            }

            public void DragOver(DropInfo dropInfo)
            {
                if (dropInfo.GetEnumerableData<TankUnikey>().Count() > 0)
                {
                    dropInfo.Effects = DragDropEffects.Copy;

                    if (_owner.ColumnMode.Mode == StatComparer.ColumnMode.Stats)
                        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                }
                else
                {
                    dropInfo.Effects = DragDropEffects.None;
                }

            }

            public void Drop(DropInfo dropInfo)
            {
                var insertIndex = _owner.ColumnMode.Mode == StatComparer.ColumnMode.Stats 
                                ? dropInfo.InsertIndex 
                                : _owner.TanksManager.SelectedTanks.Count;
                foreach (var unikey in dropInfo.GetEnumerableData<TankUnikey>())
                {
                    var tank = _owner.TanksManager.GetTankVM(unikey);
                    if (tank != null && !_owner.TanksManager.SelectedTanks.Contains(tank))
                    {
                        _owner.TanksManager.SelectedTanks.Insert(insertIndex, tank);
                        ++insertIndex;
                    }
                }
            }
        }
    }
}

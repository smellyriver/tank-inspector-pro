namespace Smellyriver.TankInspector.Common.Wpf.Behaviors.DragDrop
{
    public interface IDropTarget
    {
        void DragOver(DropInfo dropInfo);
        void Drop(DropInfo dropInfo);
    }
}

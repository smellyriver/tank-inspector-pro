namespace Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.TreeView
{
    public sealed class LoadingChildrenTreeNodeVM : TreeNodeVM
    {
        public LoadingChildrenTreeNodeVM(TreeNodeVM parent)
            : base(parent, null, LoadChildenStrategy.Manual)
        {

        }
    }
}

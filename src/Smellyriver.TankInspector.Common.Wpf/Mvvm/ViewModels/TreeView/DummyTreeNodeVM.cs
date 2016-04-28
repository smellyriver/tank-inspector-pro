namespace Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.TreeView
{
    public sealed class DummyTreeNodeVM : TreeNodeVM
    {
        public DummyTreeNodeVM(TreeNodeVM parent)
            : base(parent, "Dummy", LoadChildenStrategy.Manual)
        {

        }
    }
}

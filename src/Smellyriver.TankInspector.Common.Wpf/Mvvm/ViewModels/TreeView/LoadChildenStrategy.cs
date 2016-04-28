namespace Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.TreeView
{
    public enum LoadChildenStrategy
    {
        Manual = InternalLoadChildenStrategy.Static,
        LazyStatic = InternalLoadChildenStrategy.Lazy | InternalLoadChildenStrategy.Static,
        LazyDynamic = InternalLoadChildenStrategy.Lazy | InternalLoadChildenStrategy.Dynamic,
        LazyStaticAsync = LazyStatic | InternalLoadChildenStrategy.Async,
        LazyDynamicAsync = LazyDynamic | InternalLoadChildenStrategy.Async
    }
}

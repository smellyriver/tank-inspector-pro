namespace Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.TreeView
{
    internal class InternalLoadChildenStrategy
    {
        public const LoadChildenStrategy Lazy = (LoadChildenStrategy)0x2;
        public const LoadChildenStrategy Static = (LoadChildenStrategy)0x0;
        public const LoadChildenStrategy Dynamic = (LoadChildenStrategy)0x1;
        public const LoadChildenStrategy Async = (LoadChildenStrategy)0x4;

        public bool IsLazy { get; private set; }
        public bool IsDynamic { get; private set; }
        public bool IsAsync { get; private set; }

        public InternalLoadChildenStrategy(LoadChildenStrategy strategy)
        {
            this.IsLazy = (strategy & InternalLoadChildenStrategy.Lazy) == InternalLoadChildenStrategy.Lazy;
            this.IsDynamic = this.IsLazy && (strategy & InternalLoadChildenStrategy.Dynamic) == InternalLoadChildenStrategy.Dynamic;
            this.IsAsync = this.IsLazy && (strategy & InternalLoadChildenStrategy.Async) == InternalLoadChildenStrategy.Async;
        }
    }
}

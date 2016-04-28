namespace Smellyriver.TankInspector.Common.Utilities
{
    public sealed class SharedPair<T>
    {
        public T Source { get; private set; }
        public T Target { get; private set; }

        internal SharedPair(T source, T target)
        {
            this.Source = source;
            this.Target = target;
        }
    }
}

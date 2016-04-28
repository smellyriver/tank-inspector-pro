namespace Smellyriver.TankInspector.Common.Utilities
{
    public sealed class DiffResult<T>
    {

        public T[] Added { get; private set; }
        public T[] Removed { get; private set; }
        public SharedPair<T>[] Shared { get; private set; }
        internal DiffResult(T[] added, T[] removed, SharedPair<T>[] shared)
        {
            this.Added = added;
            this.Removed = removed;
            this.Shared = shared;
        }
    }
}

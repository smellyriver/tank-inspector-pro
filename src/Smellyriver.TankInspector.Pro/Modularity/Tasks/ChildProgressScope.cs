namespace Smellyriver.TankInspector.Pro.Modularity.Tasks
{
    public sealed class ChildProgressScope : ProgressScope
    {
        private readonly ProgressScope _parent;

        public double Weight { get; private set; }

        internal ChildProgressScope(string name, ProgressScope parent, double weight)
            : base(name)
        {
            _parent = parent;
            this.Weight = weight;
        }

        protected override void OnProgressChanged()
        {
            if (this.IsDisposed)
                return;

            base.OnProgressChanged();

            _parent.ChildReportProgress(this);
        }

        protected override void OnIsIndetermineChanged()
        {
            if (this.IsDisposed)
                return;

            base.OnIsIndetermineChanged();

            _parent.ChildReportIsIndetermine();
        }

        protected override void OnStatusMessageChanged()
        {
            if (this.IsDisposed)
                return;

            base.OnStatusMessageChanged();

            _parent.ChildReportStatusMessage(this.StatusMessage);
        }


    }
}

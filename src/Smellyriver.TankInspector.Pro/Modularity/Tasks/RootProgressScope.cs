using System;

namespace Smellyriver.TankInspector.Pro.Modularity.Tasks
{
    public sealed class RootProgressScope : ProgressScope
    {
        public event EventHandler ProgressChanged;
        public event EventHandler StatusMessageChanged;
        public event EventHandler IndetermineChanged;

        internal RootProgressScope(string name)
            : base(name)
        {

        }

        protected override void OnProgressChanged()
        {
            if (this.IsDisposed)
                return;

            base.OnProgressChanged();

            if (this.ProgressChanged != null)
                this.ProgressChanged(this, EventArgs.Empty);
        }

        protected override void OnIsIndetermineChanged()
        {
            if (this.IsDisposed)
                return;

            base.OnIsIndetermineChanged();

            if (this.IndetermineChanged != null)
                this.IndetermineChanged(this, EventArgs.Empty);
        }

        protected override void OnStatusMessageChanged()
        {
            if (this.IsDisposed)
                return;

            base.OnStatusMessageChanged();

            if (this.StatusMessageChanged != null)
                this.StatusMessageChanged(this, EventArgs.Empty);
        }

    }
}

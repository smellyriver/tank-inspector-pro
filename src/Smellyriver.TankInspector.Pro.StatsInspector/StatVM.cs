using System.Threading.Tasks;
using Smellyriver.TankInspector.Pro.Data.Stats;
using Smellyriver.TankInspector.Pro.Data.Tank;

namespace Smellyriver.TankInspector.Pro.StatsInspector
{
    class StatVM : StatVMBase
    {

        private bool _shouldShow;
        public bool ShouldShow
        {
            get { return _shouldShow; }
            private set
            {
                _shouldShow = value;
                this.RaisePropertyChanged(() => this.ShouldShow);
            }
        }

        public StatVM(IStat stat, TankInstance tank)
            : base(stat, tank)
        {
            // must be true to ensure the document to be generated correctly
            this.ShouldShow = true;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            Task.Factory.StartNew(() => this.ShouldShow = this.Model.ShouldShowFor(this.Tank, this.Tank.Repository));
        }
    }
}

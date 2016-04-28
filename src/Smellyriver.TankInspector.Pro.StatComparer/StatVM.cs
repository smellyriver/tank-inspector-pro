using System.Collections.Generic;
using System.Windows.Media;
using Smellyriver.TankInspector.Pro.Data.Stats;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.StatsShared;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    class StatVM : StatVMBase
    {
        private static readonly Dictionary<string, ImageSource> s_benchmarkIcons
            = new Dictionary<string, ImageSource>();


        private bool _isBest;
        public bool IsBest
        {
            get { return _isBest; }
            set
            {
                _isBest = value;
                this.RaisePropertyChanged(() => this.IsBest);
            }
        }

        private bool _isWorst;
        public bool IsWorst
        {
            get { return _isWorst; }
            set
            {
                _isWorst = value;
                this.RaisePropertyChanged(() => this.IsWorst);
            }
        }

        private ImageSource _benchmarkIcon;
        public ImageSource BenchmarkIcon
        {
            get { return _benchmarkIcon; }
            set
            {
                _benchmarkIcon = value;
                this.RaisePropertyChanged(() => this.BenchmarkIcon);
            }
        }


        public IStatValueGroup Group { get; }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            private set
            {
                _isLoading = value;
                this.RaisePropertyChanged(() => this.IsLoading);
            }
        }

        private bool _isLoaded;
        public bool IsLoaded
        {
            get { return _isLoaded; }
            private set
            {
                _isLoaded = value;
                this.RaisePropertyChanged(() => this.IsLoaded);
            }
        }


        public StatVM(IStatValueGroup group, IStat stat, TankInstance tank)
            : base(stat, tank)
        {
            this.Group = group;
            this.IsLoading = true;
        }

        protected override void OnLoading()
        {
            base.OnLoading();
            this.IsLoaded = false;
            this.IsLoading = true;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.IsLoaded = true;
            this.IsLoading = false;
            if (this.Group != null)
                this.Group.NotifyStatValueLoaded();
        }

        public ImageSource GetBenchmarkIcon(StatVM benchmark)
        {
            if (benchmark == null)
                return null;
            else
            {
                return ComparisonIcon.GetIcon(this.Model, benchmark.Value, this.Value);
            }
        }

    }
}

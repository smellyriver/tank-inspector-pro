using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Smellyriver.TankInspector.Common;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    class StatCategoryVM : NotificationObject, IAvailableStatsTreeItem
    {
        public string Name { get; }

        public ObservableCollection<StatInfoVM> Stats { get; }

        private Visibility _visibility;
        public Visibility Visibility
        {
            get { return _visibility; }
            private set
            {
                if (_visibility != value)
                {
                    _visibility = value;
                    this.RaisePropertyChanged(() => this.Visibility);
                }
            }
        }

        public string Description
        {
            get { return null; }
        }

        public StatCategoryVM(string name)
        {
            this.Name = name;
            this.Stats = new ObservableCollection<StatInfoVM>();
            this.UpdateVisibility();
        }

        public void UpdateVisibility()
        {
            this.Visibility = this.Stats.All(s => s.IsSelected) ? Visibility.Collapsed : Visibility.Visible;
        }

        private StatCategoryVM(StatCategoryVM other)
        {
            this.Name = other.Name;
            this.Stats = new ObservableCollection<StatInfoVM>(other.Stats);
        }

        public StatCategoryVM Clone()
        {
            return new StatCategoryVM(this);
        }

    }
}

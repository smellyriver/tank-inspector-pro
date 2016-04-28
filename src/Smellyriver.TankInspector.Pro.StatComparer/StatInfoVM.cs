using System;
using System.Windows;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Pro.Data.Stats;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    class StatInfoVM : NotificationObject, IAvailableStatsTreeItem, IGridColumn, IStatValueGroup
    {
        public string Name { get { return this.Model.Name; } }
        public string Description { get { return this.Model.Description; } }

        public string NameWithUnit
        {
            get
            {
                if (string.IsNullOrEmpty(this.Model.Unit))
                    return this.Name;
                else
                    return string.Format("{0} ({1})", this.Name, this.Model.Unit);
            }
        }

        public string Key { get { return this.Model.Key; } }

        private Visibility _visibility;

        // Visibility in AvailableStatsTree
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

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                this.RaisePropertyChanged(() => this.IsSelected);
                this.Visibility = _isSelected ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public event EventHandler StatValueLoaded;

        public IStat Model { get; }
        public StatInfoVM(IStat stat)
        {
            this.Model = stat;
        }


        void IStatValueGroup.NotifyStatValueLoaded()
        {
            if (this.StatValueLoaded != null)
                this.StatValueLoaded(this, EventArgs.Empty);
        }

    }
}

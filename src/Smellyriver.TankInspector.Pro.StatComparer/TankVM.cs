using System.Windows;
using System.Windows.Media;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Media;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Pro.UserInterface;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    class TankVM : NotificationObject, IGridColumn
    {
        private Visibility _visibility;
        // Visibility in TankMuseumTree
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
                this.UpdateVisiblity();
            }
        }

        private bool _isFilteredOut;
        public bool IsFilteredOut
        {
            get { return _isFilteredOut; }
            set
            {
                _isFilteredOut = value;
                this.RaisePropertyChanged(() => this.IsFilteredOut);
                this.UpdateVisiblity();
            }
        }

        public TankUnikey TankUnikey { get; }
        public string Key { get; }

        public IRepository Repository { get; }
        public Tank Model { get; }

        public string Name { get { return this.Model["userString"]; } }

        public ImageSource RepositoryMarker { get { return this.Repository.GetMarker(); } }
        public ImageSource Icon { get { return ApplicationImages.TryGetTankClassIcon(this.Model["class/@key"]); } }

        private bool _isBenchmark;
        public bool IsBenchmark
        {
            get { return _isBenchmark; }
            set
            {
                _isBenchmark = value;
                this.RaisePropertyChanged(() => this.IsBenchmark);
            }
        }


        private TankVM()
        {
            this.IsFilteredOut = false;
            this.IsSelected = false;
        }

        public TankVM(IRepository repository, IXQueryable tankData)
            : this()
        {
            this.TankUnikey = new TankUnikey(repository, tankData);
            this.Key = this.TankUnikey.ToString();

            this.Repository = repository;
            this.Model = new Tank(tankData);
        }

        private void UpdateVisiblity()
        {
            this.Visibility = (this.IsSelected || this.IsFilteredOut)
                            ? Visibility.Collapsed
                            : Visibility.Visible;
        }


    }
}

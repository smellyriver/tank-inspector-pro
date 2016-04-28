using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.CustomizationConfigurator
{
    class CustomizationConfigVM : NotificationObject
    {
        private CustomizationConfiguration _configuration;
        public CustomizationConfiguration Configuration
        {
            get { return _configuration; }
            set
            {
                _configuration = value;

                this.RaisePropertyChanged(() => this.Configuration);

                if (_configuration != null)
                    this.InitializeCustomizations();
            }
        }

        public IRepository Repository { get; set; }


        private CamouflageVM[] _camouflagesSource;

        private ICollectionView _camouflages;
        public ICollectionView Camouflages
        {
            get { return _camouflages; }
            private set
            {
                _camouflages = value;
                this.RaisePropertyChanged(() => this.Camouflages);
            }
        }

        private CamouflageVM _savedSelectedCamouflage;

        private CamouflageVM _selectedCamouflage;
        public CamouflageVM SelectedCamouflage
        {
            get { return _selectedCamouflage; }
            set
            {
                _selectedCamouflage = value;
                this.Configuration.Camouflage = value == null ? null : value.Model;
                this.RaisePropertyChanged(() => this.SelectedCamouflage);
            }
        }

        private bool _isCamouflageEnabled;
        public bool IsCamouflageEnabled
        {
            get { return _isCamouflageEnabled; }
            set
            {
                _isCamouflageEnabled = value;

                if (!_isCamouflageEnabled)
                {
                    _savedSelectedCamouflage = this.SelectedCamouflage;
                    this.SelectedCamouflage = null;
                }
                else
                {
                    this.SelectedCamouflage = _savedSelectedCamouflage ?? _camouflagesSource.FirstOrDefault();
                }

                this.RaisePropertyChanged(() => this.IsCamouflageEnabled);
            }
        }


        private void InitializeCustomizations()
        {
            var database = NationalCustomizationDatabase.GetDatabase(this.Repository, this.Configuration.Tank.NationKey);

            _camouflagesSource = database.CamouflageGroups
                                         .Values
                                         .SelectMany(g => g.Camouflages)
                                         .Select(g => new CamouflageVM(g, database.GetArmorColor(), this.Repository))
                                         .Where(g => g.PreviewImage != null)
                                         .ToArray();

            this.Camouflages = CollectionViewSource.GetDefaultView(_camouflagesSource);
            this.Camouflages.GroupDescriptions.Add(new PropertyGroupDescription("GroupName"));

            if (this.Configuration.Camouflage == null)
                this.IsCamouflageEnabled = false;
            else
            {
                _savedSelectedCamouflage = _camouflagesSource.FirstOrDefault(c => c.Model == this.Configuration.Camouflage);
                this.IsCamouflageEnabled = true;
            }
        }


    }
}

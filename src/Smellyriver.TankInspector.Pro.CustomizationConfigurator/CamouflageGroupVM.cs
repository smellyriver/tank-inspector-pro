using System;
using System.Linq;
using System.Windows.Media;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.CustomizationConfigurator
{
    class CamouflageGroupVM : NotificationObject
    {
        public string Name { get { return _model.Name; } }
        public CamouflageVM[] Camouflages { get; private set; }

        public event EventHandler SelectedCamouflageChanged;

        private CamouflageVM _selectedCamouflage;

        public CamouflageVM SelectedCamouflage
        {
            get { return _selectedCamouflage; }
            set
            {
                _selectedCamouflage = value;
                this.RaisePropertyChanged(() => this.SelectedCamouflage);
                if (this.SelectedCamouflageChanged != null)
                    this.SelectedCamouflageChanged(this, EventArgs.Empty);
            }
        }



        private readonly CamouflageGroup _model;
        public CamouflageGroup Model
        {
            get { return _model; }
        }

        public CamouflageGroupVM(CamouflageGroup group, Color baseColor, string tankKey, IRepository repository)
        {
            _model = group;
            this.Camouflages = group.Camouflages
                .Where(c => !c.GetIsDenied(tankKey))
                .Select(c => new CamouflageVM(c, baseColor, repository)).ToArray();
        }

    }
}

using System;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Modularity.Features;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    partial class StatComparisonDocumentVM : ITankConfigurable, ICrewConfigurable
    {

        private CrewConfiguration _crewConfiguration;
        private CrewConfiguration CrewConfiguration
        {
            get { return _crewConfiguration; }
            set
            {
                _crewConfiguration = value;
                if (this.CrewConfigurationChanged != null)
                    this.CrewConfigurationChanged(this, EventArgs.Empty);
            }
        }

        private TankConfiguration _tankConfiguration;
        private TankConfiguration TankConfiguration
        {
            get { return _tankConfiguration; }
            set
            {
                _tankConfiguration = value;
                if (this.TankConfigurationChanged != null)
                    this.TankConfigurationChanged(this, EventArgs.Empty);
            }
        }

        TankConfiguration ITankConfigurable.TankConfiguration
        {
            get { return this.TankConfiguration; }
        }

        public event EventHandler TankConfigurationChanged;

        CrewConfiguration ICrewConfigurable.CrewConfiguration
        {
            get { return this.CrewConfiguration; }
        }

        public event EventHandler CrewConfigurationChanged;

        IRepository ITankConfigurable.Repository
        {
            get { return this.TanksManager.SelectedTankRepository; }
        }

        IRepository ICrewConfigurable.Repository
        {
            get { return this.TanksManager.SelectedTankRepository; }
        }
    }
}

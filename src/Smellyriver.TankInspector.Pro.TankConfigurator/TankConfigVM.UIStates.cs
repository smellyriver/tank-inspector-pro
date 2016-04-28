using System.ComponentModel;

namespace Smellyriver.TankInspector.Pro.TankConfigurator
{
    partial class TankConfigVM
    {
        public bool IsModulesExpanded
        {
            get { return TankConfiguratorSettings.Default.ModulesExpanded; }
            set
            {
                TankConfiguratorSettings.Default.ModulesExpanded = value;
                TankConfiguratorSettings.Default.Save();
            }
        }

        public bool IsAmmunitionExpanded
        {
            get { return TankConfiguratorSettings.Default.AmmunitionExpanded; }
            set
            {
                TankConfiguratorSettings.Default.AmmunitionExpanded = value;
                TankConfiguratorSettings.Default.Save();
            }
        }

        public bool IsEquipmentsExpanded
        {
            get { return TankConfiguratorSettings.Default.EquipmentsExpanded; }
            set
            {
                TankConfiguratorSettings.Default.EquipmentsExpanded = value;
                TankConfiguratorSettings.Default.Save();
            }
        }

        public bool IsConsumablesExpanded
        {
            get { return TankConfiguratorSettings.Default.ConsumablesExpanded; }
            set
            {
                TankConfiguratorSettings.Default.ConsumablesExpanded = value;
                TankConfiguratorSettings.Default.Save();
            }
        }

        void OnTankConfiguratorSettingChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "ModulesExpanded":
                    this.RaisePropertyChanged(() => this.IsModulesExpanded);
                    break;
                case "AmmunitionExpanded":
                    this.RaisePropertyChanged(() => this.IsAmmunitionExpanded);
                    break;
                case "EquipmentsExpanded":
                    this.RaisePropertyChanged(() => this.IsEquipmentsExpanded);
                    break;
                case "ConsumablesExpanded":
                    this.RaisePropertyChanged(() => this.IsConsumablesExpanded);
                    break;
            }
        }
    }
}

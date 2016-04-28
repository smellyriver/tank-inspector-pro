using System;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Features;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;

namespace Smellyriver.TankInspector.Pro.TankConfigurator
{
    [ModuleExport("TankConfiguratorModule", typeof(TankConfiguratorModule))]
    [ExportMetadata("Guid", "D2D8A197-E585-42C2-BE95-BEAF43D01B60")]
    [ExportMetadata("Name", "#tank_configurator:module_name")]
    [ExportMetadata("Description", "#tank_configurator:module_description")]
    [ExportMetadata("Version", "1.0.0.0")]
    [ExportMetadata("Provider", "#tank_configurator:module_provider")]
    public class TankConfiguratorModule : ModuleBase
    {

        private TankConfigView _configView;
        private TankConfigVM _configVm;


        private ITankConfigurable _tankConfigurable;
        private ITankConfigurable TankConfigurable
        {
            get { return _tankConfigurable; }
            set
            {
                if (_tankConfigurable != null)
                {
                    _tankConfigurable.TankConfigurationChanged -= OnTankConfigurationChanged;
                }

                _tankConfigurable = value;

                if (_tankConfigurable != null)
                {
                    this.UpdateConfiguration();
                    _tankConfigurable.TankConfigurationChanged += OnTankConfigurationChanged;
                }
                else
                    _configVm.Configuration = null;

            }
        }

        public override void Initialize()
        {
            _configView = new TankConfigView();
            _configVm = new TankConfigVM();
            _configView.ViewModel = _configVm;

            var panel = new FeaturedPanelInfo(
               Guid.Parse("090B35D3-1760-4A37-8589-4F8C66A8D30B"),
               new[] { typeof(ITankConfigurable) },
               this.OnRequiredFeaturesSatisficationChanged)
            {
                Title = this.L("tank_configurator", "panel_title"),
                CanHide = true,
                CanClose = true,
                CanFloat = true,
                Width = 200,
                Content = _configView,
                IconSource = BitmapImageEx.LoadAsFrozen("Resources/Images/Config_16.png"),
            };

            DockingViewManager.Instance.PanelManager.Register(panel);
        }

        private void OnRequiredFeaturesSatisficationChanged(DocumentInfo document, bool satisified)
        {
            _configView.IsEnabled = satisified;

            if (satisified)
            {
                this.TankConfigurable = document.GetFeature<ITankConfigurable>();
            }
            else
            {
                this.TankConfigurable = null;
            }
        }


        void OnTankConfigurationChanged(object sender, EventArgs e)
        {
            this.UpdateConfiguration();
        }

        private void UpdateConfiguration()
        {
            _configVm.Repository = _tankConfigurable.Repository;
            _configVm.Configuration = _tankConfigurable.TankConfiguration;
            _configView.IsEnabled = _tankConfigurable.TankConfiguration != null;
        }
    }
}

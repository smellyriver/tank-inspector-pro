using System;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Features;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;

namespace Smellyriver.TankInspector.Pro.CrewConfigurator
{
    [ModuleExport("CrewConfigurator", typeof(CrewConfiguratorModule))]
    [ExportMetadata("Guid", "C229BE91-98D2-479C-9199-19EFF9962EC3")]
    [ExportMetadata("Name", "#crew_configurator:module_name")]
    [ExportMetadata("Description", "#crew_configurator:module_description")]
    [ExportMetadata("Version", "1.0.0.0")]
    [ExportMetadata("Provider", "#crew_configurator:module_provider")]
    public class CrewConfiguratorModule : ModuleBase
    {

        private CrewConfigView _configView;
        private CrewConfigVM _configVm;

        private ICrewConfigurable _crewConfigurable;
        private ICrewConfigurable CrewConfigurable
        {
            get { return _crewConfigurable; }
            set
            {
                if (_crewConfigurable != null)
                {
                    _crewConfigurable.CrewConfigurationChanged -= OnCrewConfigurationChanged;
                }

                _crewConfigurable = value;

                if (_crewConfigurable != null)
                {
                    this.UpdateConfiguration();
                    _crewConfigurable.CrewConfigurationChanged += OnCrewConfigurationChanged;
                }
                else
                    _configVm.Configuration = null;

            }
        }

        public override void Initialize()
        {
            _configView = new CrewConfigView();
            _configVm = new CrewConfigVM();
            _configView.ViewModel = _configVm;

            var panel = new FeaturedPanelInfo(
               Guid.Parse("ADCC6DC1-160B-48D3-BAD0-12D624C4514C"),
               new[] { typeof(ICrewConfigurable) },
               this.OnRequiredFeaturesSatisficationChanged)
            {
                Title = this.L("crew_configurator", "panel_title"),
                CanHide = true,
                CanClose = true,
                CanFloat = true,
                Width = 200,
                Content = _configView,
                IconSource = BitmapImageEx.LoadAsFrozen("Resources/Images/Crew_16.png"),
            };

            DockingViewManager.Instance.PanelManager.Register(panel);
        }

        private void OnRequiredFeaturesSatisficationChanged(DocumentInfo document, bool satisified)
        {
            _configView.IsEnabled = satisified;

            if (satisified)
            {
                this.CrewConfigurable = document.GetFeature<ICrewConfigurable>();
            }
            else
            {
                this.CrewConfigurable = null;
            }
        }


        void OnCrewConfigurationChanged(object sender, EventArgs e)
        {
            this.UpdateConfiguration();
        }


        private void UpdateConfiguration()
        {
            _configVm.Repository = _crewConfigurable.Repository;
            _configVm.Configuration = _crewConfigurable.CrewConfiguration;
            _configView.IsEnabled =  _crewConfigurable.CrewConfiguration!= null;
        }
    }
}

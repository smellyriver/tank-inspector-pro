using System;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Features;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;

namespace Smellyriver.TankInspector.Pro.CustomizationConfigurator
{
    [ModuleExport("CustomizationConfigurator", typeof(CustomizationConfiguratorModule))]
    [ExportMetadata("Guid", "4DBA9554-77F0-4291-BD47-C0F2CB4EE6A7")]
    [ExportMetadata("Name", "#customization_configurator:module_name")]
    [ExportMetadata("Description", "#customization_configurator:module_description")]
    [ExportMetadata("Version", "1.0.0.0")]
    [ExportMetadata("Provider", "#customization_configurator:module_provider")]
    public class CustomizationConfiguratorModule : ModuleBase
    {

        private CustomizationConfigView _configView;
        private CustomizationConfigVM _configVm;

        private ICustomizationConfigurable _customizationConfigurable;
        private ICustomizationConfigurable CustomizationConfigurable
        {
            get { return _customizationConfigurable; }
            set
            {
                if (_customizationConfigurable != null)
                {
                    _customizationConfigurable.CustomizationConfigurationChanged -= OnCustomizationConfigurationChanged;
                }

                _customizationConfigurable = value;

                if (_customizationConfigurable != null)
                {
                    this.UpdateConfiguration();
                    _customizationConfigurable.CustomizationConfigurationChanged += OnCustomizationConfigurationChanged;
                }
                else
                    _configVm.Configuration = null;

            }
        }

        public override void Initialize()
        {
            _configView = new CustomizationConfigView();
            _configVm = new CustomizationConfigVM();
            _configView.ViewModel = _configVm;

            var panel = new FeaturedPanelInfo(
               Guid.Parse("D7C7755E-47E7-4EB0-8904-2FD24D181CDB"),
               new[] { typeof(ICustomizationConfigurable) },
               this.OnRequiredFeaturesSatisficationChanged)
            {
                Title = this.L("customization_configurator", "panel_title"),
                CanHide = true,
                CanClose = true,
                CanFloat = true,
                Width = 200,
                Content = _configView,
                IconSource = BitmapImageEx.LoadAsFrozen("Resources/Images/Pencil_16.png"),
            };

            DockingViewManager.Instance.PanelManager.Register(panel);
        }

        private void OnRequiredFeaturesSatisficationChanged(DocumentInfo document, bool satisified)
        {
            _configView.IsEnabled = satisified;

            if (satisified)
            {
                this.CustomizationConfigurable = document.GetFeature<ICustomizationConfigurable>();
            }
            else
            {
                this.CustomizationConfigurable = null;
            }
        }


        void OnCustomizationConfigurationChanged(object sender, EventArgs e)
        {
            this.UpdateConfiguration();
        }


        private void UpdateConfiguration()
        {
            _configVm.Repository = _customizationConfigurable.Repository;
            _configVm.Configuration = _customizationConfigurable.CustomizationConfiguration;
            _configView.IsEnabled = _customizationConfigurable.CustomizationConfiguration != null;
        }
    }
}

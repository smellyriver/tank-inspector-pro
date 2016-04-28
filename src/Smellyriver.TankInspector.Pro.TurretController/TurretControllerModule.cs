using System;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Features;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;

namespace Smellyriver.TankInspector.Pro.TurretController
{

    [ModuleExport("TurretController", typeof(TurretControllerModule))]
    [ExportMetadata("Guid", "49DBD696-0FE0-4BC3-8D8D-5A614DFACC6B")]
    [ExportMetadata("Name", "#turret_controller:module_name")]
    [ExportMetadata("Description", "#turret_controller:module_description")]
    [ExportMetadata("Version", "1.0.0.0")]
    [ExportMetadata("Provider", "#turret_controller:module_provider")]
    public class TurretControllerModule : ModuleBase
    {
        private TurretControllerView _controllerView;
        private TurretControllerVM _controllerVm;

        private ITurretControllable _turretControllable;
        private ITurretControllable TurretControllable
        {
            get { return _turretControllable; }
            set
            {
                if (_turretControllable != null)
                {
                    _turretControllable.TankInstanceChanged -= OnTankInstanceChanged;
                }

                _turretControllable = value;

                if (_turretControllable != null)
                {
                    this.UpdateConfiguration();
                    _turretControllable.TankInstanceChanged += OnTankInstanceChanged;
                }
                else
                    _controllerVm.TankInstance = null;

            }
        }

        public override void Initialize()
        {

            _controllerVm = new TurretControllerVM();
            _controllerView = new TurretControllerView();
            _controllerView.ViewModel = _controllerVm;

            var panel = new FeaturedPanelInfo(
                Guid.Parse("7262E37C-3C56-4E55-81BB-30DF8ACC1E23"),
                new[] { typeof(ITurretControllable) },
                this.OnRequiredFeaturesSatisficationChanged)
            {
                Title = this.L("turret_controller", "panel_title"),
                CanHide = true,
                CanClose = true,
                CanFloat = true,
                Width = 200,
                Content = _controllerView,
                IconSource = BitmapImageEx.LoadAsFrozen("Resources/Images/Icon_16.png"),
            };

            DockingViewManager.Instance.PanelManager.Register(panel);
        }

        private void OnRequiredFeaturesSatisficationChanged(DocumentInfo document, bool satisified)
        {
            _controllerView.IsEnabled = satisified;

            if (satisified)
            {
                this.TurretControllable = document.GetFeature<ITurretControllable>();
            }
            else
            {
                this.TurretControllable = null;
            }
        }


        void OnTankInstanceChanged(object sender, EventArgs e)
        {
            this.UpdateConfiguration();
        }


        private void UpdateConfiguration()
        {
            _controllerVm.TankInstance = _turretControllable.TankInstance;
            _controllerView.IsEnabled = _turretControllable.TankInstance != null;
        }
    }
}

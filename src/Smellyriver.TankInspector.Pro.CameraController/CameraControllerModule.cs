using System;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Features;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;

namespace Smellyriver.TankInspector.Pro.CameraController
{
    [ModuleExport("CameraController", typeof(CameraControllerModule))]
    [ExportMetadata("Guid", "D3E1518B-D0C6-405E-B456-70B5E6417D73")]
    [ExportMetadata("Name", "#camera_controller:module_name")]
    [ExportMetadata("Description", "#camera_controller:module_description")]
    [ExportMetadata("Version", "1.0.0.0")]
    [ExportMetadata("Provider", "#camera_controller:module_provider")]
    public class CameraControllerModule : ModuleBase
    {

        private CameraControllerView _cameraControllerView;
        private CameraControllerVM _cameraControllerVm;

        private IHasCamera _cameraContainer;

        public IHasCamera CameraContainer
        {
            get { return _cameraContainer; }
            set
            {

                if (_cameraContainer != null)
                {
                    _cameraContainer.CameraChanged -= CameraContainer_CameraChanged;
                }

                _cameraContainer = value;

                if (_cameraContainer != null)
                {
                    _cameraContainer.CameraChanged += CameraContainer_CameraChanged;
                    this.UpdateCamera();
                }
                else
                    _cameraControllerVm.Camera = null;
            }
        }

        private void UpdateCamera()
        {
            _cameraControllerVm.Camera = this.CameraContainer.Camera;
            _cameraControllerView.IsEnabled = this.CameraContainer != null
                                              && this.CameraContainer.Camera != null;
        }

        void CameraContainer_CameraChanged(object sender, EventArgs e)
        {
            this.UpdateCamera();
        }

        public override void Initialize()
        {
            _cameraControllerVm = new CameraControllerVM();
            _cameraControllerView = new CameraControllerView();
            _cameraControllerView.ViewModel = _cameraControllerVm;

            var panel = new FeaturedPanelInfo(
                Guid.Parse("72B29BFF-9421-467B-84F8-730CC39BA8E0"),
                new[] { typeof(IHasCamera) },
                this.OnRequiredFeaturesSatisficationChanged)
            {
                Title = this.L("camera_controller", "panel_title"),
                CanHide = true,
                CanClose = true,
                CanFloat = true,
                Content = _cameraControllerView,
                IconSource = BitmapImageEx.LoadAsFrozen("Resources/Images/Camera_12.png"),
            };

            DockingViewManager.Instance.PanelManager.Register(panel);
        }

        private void OnRequiredFeaturesSatisficationChanged(DocumentInfo document, bool satisified)
        {
            _cameraControllerView.IsEnabled = satisified;

            if (satisified)
                this.CameraContainer = document.GetFeature<IHasCamera>();
            else
                this.CameraContainer = null;
        }
    }
}

using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Graphics;
using Smellyriver.TankInspector.Pro.Graphics.Scene;

namespace Smellyriver.TankInspector.Pro.ModelShared
{
    public abstract class ModelVMBase : NotificationObject
    {
        private bool _showGun;
        public bool ShowGun
        {
            get { return _showGun; }
            internal set
            {
                _showGun = value;
                this.RaisePropertyChanged(() => this.ShowGun);
            }
        }

        private bool _showTurret;
        public bool ShowTurret
        {
            get { return _showTurret; }
            internal set
            {
                _showTurret = value;
                this.RaisePropertyChanged(() => this.ShowTurret);
            }
        }

        private bool _showHull;
        public bool ShowHull
        {
            get { return _showHull; }
            internal set
            {
                _showHull = value;
                this.RaisePropertyChanged(() => this.ShowHull);
            }
        }

        private bool _showChassis;
        public bool ShowChassis
        {
            get { return _showChassis; }
            internal set
            {
                _showChassis = value;
                this.RaisePropertyChanged(() => this.ShowChassis);
            }
        }

        private bool _wireframeMode;
        public bool UseWireframeMode
        {
            get { return _wireframeMode; }
            internal set
            {
                _wireframeMode = value;
                this.GraphicsSettings.WireframeMode = value;
                this.RaisePropertyChanged(() => this.UseWireframeMode);
            }
        }

        private bool _showFps;
        public bool ShowFps
        {
            get { return _showFps; }
            internal set
            {
                _showFps = value;
                this.RaisePropertyChanged(() => this.ShowFps);
            }
        }

        private bool _showTriangleCount;
        public bool ShowTriangleCount
        {
            get { return _showTriangleCount; }
            internal set
            {
                _showTriangleCount = value;
                this.RaisePropertyChanged(() => this.ShowTriangleCount);
            }
        }

        private RotationCenter _rotationCenter;
        public RotationCenter RotationCenter
        {
            get { return _rotationCenter; }
            set
            {
                _rotationCenter = value;
                this.RaisePropertyChanged(() => this.RotationCenter);
            }
        }

        public TankInstance TankInstance { get; private set; }

        private TankInstance _alternativeTankInstance;
        public TankInstance AlternativeTankInstance
        {
            get { return _alternativeTankInstance; }
            set
            {
                _alternativeTankInstance = value;
                this.RaisePropertyChanged(() => this.AlternativeTankInstance);
            }
        }


        public GraphicsSettings GraphicsSettings { get; }

        public ModelVMBase(TankInstance tankInstance)
        {
            this.GraphicsSettings = new GraphicsSettings();
            this.TankInstance = tankInstance;
            this.ShowGun = true;
            this.ShowTurret = true;
            this.ShowChassis = true;
            this.ShowHull = true;
        }

    }
}

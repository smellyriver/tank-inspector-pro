using System.Collections.Generic;
using System.Linq;

namespace Smellyriver.TankInspector.Pro.ModelShared
{
    partial class ModelDocumentVMBase
    {

        public bool ShowGun
        {
            get { return this.ModelView.ShowGun; }
            internal set
            {
                this.ModelView.ShowGun = value;
                this.PersistentInfo.ShowGun = value;
                this.RaisePropertyChanged(() => this.ShowGun);
            }
        }

        public bool ShowTurret
        {
            get { return this.ModelView.ShowTurret; }
            internal set
            {
                this.ModelView.ShowTurret = value;
                this.PersistentInfo.ShowTurret = value;
                this.RaisePropertyChanged(() => this.ShowTurret);
            }
        }

        public bool ShowHull
        {
            get { return this.ModelView.ShowHull; }
            internal set
            {
                this.ModelView.ShowHull = value;
                this.PersistentInfo.ShowHull = value;
                this.RaisePropertyChanged(() => this.ShowHull);
            }
        }

        public bool ShowChassis
        {
            get { return this.ModelView.ShowChassis; }
            internal set
            {
                this.ModelView.ShowChassis = value;
                this.PersistentInfo.ShowChassis = value;
                this.RaisePropertyChanged(() => this.ShowChassis);
            }
        }

        public bool UseWireframeMode
        {
            get { return this.ModelView.UseWireframeMode; }
            internal set
            {
                this.ModelView.UseWireframeMode = value;
                this.PersistentInfo.UseWireframeMode = value;
                this.RaisePropertyChanged(() => this.UseWireframeMode);
                this.RaisePropertyChanged(() => this.UseSolidMode);
            }
        }

        public bool UseSolidMode { get { return !this.UseWireframeMode; } }

        public bool ShowFps
        {
            get { return this.ModelView.ShowFps; }
            internal set
            {
                this.ModelView.ShowFps = value;
                this.PersistentInfo.ShowFps = value;
                this.RaisePropertyChanged(() => this.ShowFps);
            }
        }

        public bool ShowTriangleCount
        {
            get { return this.ModelView.ShowTriangleCount; }
            internal set
            {
                this.ModelView.ShowTriangleCount = value;
                this.PersistentInfo.ShowTriangleCount = value;
                this.RaisePropertyChanged(() => this.ShowTriangleCount);
            }
        }

        private RotationCenterVM[] _rotationCenters = RotationCenterVM.RotationCenters;
        public IEnumerable<RotationCenterVM> RotationCenters { get { return _rotationCenters; } }

        public RotationCenterVM RotationCenter
        {
            get { return _rotationCenters.First(r => r.Model == this.ModelView.RotationCenter); }
            set
            {
                this.ModelView.RotationCenter = value.Model;
                this.PersistentInfo.RotationCenter = value.Model;
                this.RaisePropertyChanged(() => this.RotationCenter);
            }
        }
    }
}

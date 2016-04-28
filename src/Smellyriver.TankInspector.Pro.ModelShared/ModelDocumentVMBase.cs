using System;
using System.Windows.Input;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Graphics;
using Smellyriver.TankInspector.Pro.Graphics.Controls;
using Smellyriver.TankInspector.Pro.Graphics.Scene;
using Smellyriver.TankInspector.Pro.Modularity.Features;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.ModelShared
{
    public abstract partial class ModelDocumentVMBase : NotificationObject, ITankConfigurable, IHasCamera, ITurretControllable
    {
        public ModelVMBase ModelView { get; }

        private readonly CommandBindingCollection _commandBindings;
        protected CommandBindingCollection CommandBindings { get { return _commandBindings; } }

        private TankInstance _tankInstance;
        public TankInstance TankInstance
        {
            get { return _tankInstance; }
            set
            {
                _tankInstance = value;
                this.RaisePropertyChanged(() => this.TankInstance);
            }
        }


        internal protected ModelDocumentPersistentInfoBase PersistentInfo { get; }

        TankConfiguration ITankConfigurable.TankConfiguration
        {
            get { return this.TankInstance.TankConfiguration; }
        }

        event EventHandler ITankConfigurable.TankConfigurationChanged { add { } remove { } }
        event EventHandler ITurretControllable.TankInstanceChanged { add { } remove { } }

        public IRepository Repository
        {
            get { return this.TankInstance.Repository; }
        }

        public Camera Camera
        {
            get { return this.PersistentInfo.Camera; }
            set { this.PersistentInfo.Camera = value; }
        }

        event EventHandler IHasCamera.CameraChanged { add { } remove { } }

        public ISnapshotProvider SnapshotProvider { get; set; }
        public ISnapshotProvider AlternativeSnapshotProvider { get; set; }


        private HangarSceneSource _hangarSceneSource;
        public HangarSceneSource HangarSceneSource
        {
            get { return _hangarSceneSource; }
            set
            {
                _hangarSceneSource = value;
                this.RaisePropertyChanged(() => this.HangarSceneSource);
            }
        }

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


        public ModelDocumentVMBase(CommandBindingCollection commandBindings, TankInstance tankInstance, string persistentInfo)
        {
            this.ModelView = this.CreateModelVM(tankInstance);
            this.TankInstance = tankInstance;
            this.PersistentInfo = this.LoadPersistentInfo(persistentInfo);
            _commandBindings = commandBindings;
            this.InitializeCommands();

            this.Camera.RotationYawChanged += Camera_RotationYawChanged;
        }

        void Camera_RotationYawChanged(object sender, EventArgs e)
        {
            this.TankInstance.Transform.VehicleYaw = this.Camera.RotationYaw - 90;
        }

        protected abstract ModelVMBase CreateModelVM(TankInstance tankInstance);
        protected abstract ModelDocumentPersistentInfoBase LoadPersistentInfo(string persistentInfo);



    }
}

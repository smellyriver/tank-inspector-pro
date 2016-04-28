using System;
using System.Windows;
using System.Windows.Controls;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.Pro.Graphics.Frameworks;
using Smellyriver.TankInspector.Pro.Graphics.Scene;

namespace Smellyriver.TankInspector.Pro.Graphics.Controls
{
    /// <summary>
    /// Interaction logic for ModelView.xaml
    /// </summary>
    public partial class ModelView : UserControl
    {
        public ISnapshotProvider SnapshotProvider { get { return this.HangarScene; } }
        public ISnapshotProvider AlternativeSnapshotProvider { get { return _alternativeHangarScene; } }

        private DXElement _alternativeDXElement;
        private HangarScene _alternativeHangarScene;

        public event EventHandler AlternativeHangarSceneChanged;

        public ModelView()
        {
            InitializeComponent();

            this.InitializeHangarScene(this.HangarScene);

            this.HangarScene.AddPropertyChangedHandler(HangarScene.IsInitializingProperty, this.OnHangarSceneIsInitializingChanged);
        }

        private void OnHangarSceneIsInitializingChanged(object sender, EventArgs e)
        {
            this.IsInitializing = this.HangarScene.IsInitializing;
        }

        private void InitializeHangarScene(HangarScene hangarScene)
        {
            hangarScene.ShowGun = this.ShowGun;
            hangarScene.ShowTurret = this.ShowTurret;
            hangarScene.ShowChassis = this.ShowChassis;
            hangarScene.ShowHull = this.ShowHull;
            hangarScene.ProjectionMode = this.ProjectionMode;
            hangarScene.Fov = this.Fov;
            hangarScene.ShowCamouflage = this.ShowCamouflage;
            hangarScene.FileSource = this.FileSource;
            hangarScene.ModelType = this.ModelType;
            hangarScene.RotationCenter = this.RotationCenter;
        }

        private void InitializeAlternativeView()
        {
            _alternativeDXElement = new DXElement();
            _alternativeHangarScene = new HangarScene();
            _alternativeHangarScene.Renderer = new D3D9();
            _alternativeDXElement.Renderer = _alternativeHangarScene;
            _alternativeDXElement.Visibility = Visibility.Hidden;
            this.InitializeHangarScene(_alternativeHangarScene);
            this.AlternativeDXElementContainer.Children.Add(_alternativeDXElement);
            this.OnAlternativeHangarSceneChanged();
        }

        private void RemoveAlternativeView()
        {
            this.AlternativeDXElementContainer.Children.Clear();
            _alternativeDXElement = null;
            _alternativeHangarScene = null;
            this.OnAlternativeHangarSceneChanged();
        }

        private void OnAlternativeHangarSceneChanged()
        {
            if (this.AlternativeHangarSceneChanged != null)
                this.AlternativeHangarSceneChanged(this, EventArgs.Empty);
        }

        private void ConfigHangarScene(Action<HangarScene> action)
        {
            action(this.HangarScene);
            if (_alternativeHangarScene != null)
                action(_alternativeHangarScene);
        }
    }
}

using System;
using System.Windows;
using System.Windows.Media.Media3D;
using SharpDX;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Graphics.Frameworks;
using Smellyriver.TankInspector.Pro.Graphics.Scene;
using Smellyriver.TankInspector.Pro.IO;

namespace Smellyriver.TankInspector.Pro.Graphics.Controls
{
    partial class ModelView
    {
        public RenderActivityLevel RenderActivityLevel
        {
            get { return (RenderActivityLevel)GetValue(RenderActivityLevelProperty); }
            set { SetValue(RenderActivityLevelProperty, value); }
        }

        public static readonly DependencyProperty RenderActivityLevelProperty =
            DependencyProperty.Register("RenderActivityLevel",
                                        typeof(RenderActivityLevel),
                                        typeof(ModelView),
                                        new PropertyMetadata(RenderActivityLevel.Normal, ModelView.OnRenderActivityLevelChanged));

        private static void OnRenderActivityLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModelView)d).OnRenderActivityLevelChanged((RenderActivityLevel)e.NewValue);
        }



        public TankInstance TankInstance
        {
            get { return (TankInstance)GetValue(TankInstanceProperty); }
            set { SetValue(TankInstanceProperty, value); }
        }

        public static readonly DependencyProperty TankInstanceProperty =
            DependencyProperty.Register("TankInstance",
                                        typeof(TankInstance),
                                        typeof(ModelView),
                                        new PropertyMetadata(null, ModelView.OnTankInstanceChanged));

        private static void OnTankInstanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModelView)d).OnTankInstanceChanged((TankInstance)e.NewValue);
        }

        public ModelType ModelType
        {
            get { return (ModelType)GetValue(ModelTypeProperty); }
            set { SetValue(ModelTypeProperty, value); }
        }

        public static readonly DependencyProperty ModelTypeProperty =
            DependencyProperty.Register("ModelType",
                                        typeof(ModelType),
                                        typeof(ModelView),
                                        new PropertyMetadata(ModelType.Undamaged, ModelView.OnModelTypeChanged));



        private static void OnModelTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModelView)d).OnModelTypeChanged((ModelType)e.NewValue);
        }

        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        public static readonly DependencyProperty ZoomProperty =
            DependencyProperty.Register("Zoom", typeof(double), typeof(ModelView), new PropertyMetadata(1.0, ModelView.OnZoomChanged));

        private static void OnZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModelView)d).OnZoomChanged((double)e.NewValue);
        }


        public Transform3D CameraTransform
        {
            get { return (Transform3D)GetValue(CameraTransformProperty); }
            set { SetValue(CameraTransformProperty, value); }
        }

        public static readonly DependencyProperty CameraTransformProperty =
            DependencyProperty.Register("CameraTransform",
                                        typeof(Transform3D),
                                        typeof(ModelView),
                                        new PropertyMetadata(Transform3D.Identity, ModelView.OnCameraTransformChanged));

        private static void OnCameraTransformChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModelView)d).OnCameraTransformChanged((Transform3D)e.NewValue);
        }

        public Vector3D LookDirection
        {
            get { return (Vector3D)GetValue(LookDirectionProperty); }
            set { SetValue(LookDirectionProperty, value); }
        }

        public static readonly DependencyProperty LookDirectionProperty =
            DependencyProperty.Register("LookDirection",
                                        typeof(Vector3D),
                                        typeof(ModelView),
                                        new PropertyMetadata(new Vector3D(), ModelView.OnLookDirectionChanged));

        private static void OnLookDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModelView)d).OnLookDirectionChanged((Vector3D)e.NewValue);
        }

        public GraphicsSettings GraphicsSettings
        {
            get { return (GraphicsSettings)GetValue(GraphicsSettingsProperty); }
            set { SetValue(GraphicsSettingsProperty, value); }
        }

        public static readonly DependencyProperty GraphicsSettingsProperty =
            DependencyProperty.Register("GraphicsSettings",
                                        typeof(GraphicsSettings),
                                        typeof(ModelView),
                                        new PropertyMetadata(GraphicsSettings.Default, ModelView.OnGraphicsSettingsChanged));

        private static void OnGraphicsSettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModelView)d).OnGraphicsSettingsChanged((GraphicsSettings)e.NewValue);
        }

        public bool ShowGun
        {
            get { return (bool)GetValue(ShowGunProperty); }
            set { SetValue(ShowGunProperty, value); }
        }

        public static readonly DependencyProperty ShowGunProperty =
            DependencyProperty.Register("ShowGun", typeof(bool), typeof(ModelView), new PropertyMetadata(true, ModelView.OnShowGunChanged));

        private static void OnShowGunChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModelView)d).OnShowGunChanged((bool)e.NewValue);
        }

        public bool ShowTurret
        {
            get { return (bool)GetValue(ShowTurretProperty); }
            set { SetValue(ShowTurretProperty, value); }
        }

        public static readonly DependencyProperty ShowTurretProperty =
            DependencyProperty.Register("ShowTurret", typeof(bool), typeof(ModelView), new PropertyMetadata(true, ModelView.OnShowTurretChanged));

        private static void OnShowTurretChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModelView)d).OnShowTurretChanged((bool)e.NewValue);
        }

        public bool ShowHull
        {
            get { return (bool)GetValue(ShowHullProperty); }
            set { SetValue(ShowHullProperty, value); }
        }

        public static readonly DependencyProperty ShowHullProperty =
            DependencyProperty.Register("ShowHull", typeof(bool), typeof(ModelView), new PropertyMetadata(true, ModelView.OnShowHullChanged));

        private static void OnShowHullChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModelView)d).OnShowHullChanged((bool)e.NewValue);
        }

        public bool ShowChassis
        {
            get { return (bool)GetValue(ShowChassisProperty); }
            set { SetValue(ShowChassisProperty, value); }
        }

        public static readonly DependencyProperty ShowChassisProperty =
            DependencyProperty.Register("ShowChassis", typeof(bool), typeof(ModelView), new PropertyMetadata(true, ModelView.OnShowChassisChanged));

        private static void OnShowChassisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModelView)d).OnShowChassisChanged((bool)e.NewValue);
        }



        public CollisionModelRenderParameters CollisionModelRenderParameters
        {
            get { return (CollisionModelRenderParameters)GetValue(CollisionModelRenderParametersProperty); }
            set { SetValue(CollisionModelRenderParametersProperty, value); }
        }

        public static readonly DependencyProperty CollisionModelRenderParametersProperty =
            DependencyProperty.Register("CollisionModelRenderParameters",
                                        typeof(CollisionModelRenderParameters),
                                        typeof(ModelView),
                                        new PropertyMetadata(null, ModelView.OnCollisionModelRenderParametersChanged));

        private static void OnCollisionModelRenderParametersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModelView)d).OnCollisionModelRenderParametersChanged((CollisionModelRenderParameters)e.NewValue);
        }



        public ProjectionMode ProjectionMode
        {
            get { return (ProjectionMode)GetValue(ProjectionModeProperty); }
            set { SetValue(ProjectionModeProperty, value); }
        }

        public static readonly DependencyProperty ProjectionModeProperty =
            DependencyProperty.Register("ProjectionMode",
                                        typeof(ProjectionMode),
                                        typeof(ModelView),
                                        new PropertyMetadata(ProjectionMode.Perspective, ModelView.OnProjectionModeChanged));

        private static void OnProjectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModelView)d).OnProjectionModeChanged((ProjectionMode)e.NewValue);
        }



        public double Fov
        {
            get { return (double)GetValue(FovProperty); }
            set { SetValue(FovProperty, value); }
        }

        public static readonly DependencyProperty FovProperty =
            DependencyProperty.Register("Fov",
                                        typeof(double),
                                        typeof(ModelView),
                                        new PropertyMetadata(Math.PI / 4.0, ModelView.OnFovChanged));

        private static void OnFovChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModelView)d).OnFovChanged((double)e.NewValue);
        }



        public bool ShowCamouflage
        {
            get { return (bool)GetValue(ShowCamouflageProperty); }
            set { SetValue(ShowCamouflageProperty, value); }
        }

        public static readonly DependencyProperty ShowCamouflageProperty =
            DependencyProperty.Register("ShowCamouflage",
                                        typeof(bool),
                                        typeof(ModelView),
                                        new PropertyMetadata(false, ModelView.OnShowCamouflageChanged));

        private static void OnShowCamouflageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModelView)d).OnShowCamouflageChanged((bool)e.NewValue);
        }



        public FileSource FileSource
        {
            get { return (FileSource)GetValue(FileSourceProperty); }
            set { SetValue(FileSourceProperty, value); }
        }

        public static readonly DependencyProperty FileSourceProperty =
            DependencyProperty.Register("FileSource", typeof(FileSource), typeof(ModelView), new PropertyMetadata(FileSource.Package, ModelView.OnFileSourceChanged));

        private static void OnFileSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModelView)d).OnFileSourceChanged((FileSource)e.NewValue);
        }



        public bool IsInitializing
        {
            get { return (bool)GetValue(IsInitializingProperty.DependencyProperty); }
            private set { SetValue(IsInitializingProperty, value); }
        }

        public static readonly DependencyPropertyKey IsInitializingProperty =
            DependencyProperty.RegisterReadOnly("IsInitializing", typeof(bool), typeof(ModelView), new PropertyMetadata(true));



        public RotationCenter RotationCenter
        {
            get { return (RotationCenter)GetValue(RotationCenterProperty); }
            set { SetValue(RotationCenterProperty, value); }
        }

        public static readonly DependencyProperty RotationCenterProperty =
            DependencyProperty.Register("RotationCenter", typeof(RotationCenter), typeof(ModelView), new PropertyMetadata(RotationCenter.Hull, ModelView.OnRotationCenterChanged));

        private static void OnRotationCenterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModelView)d).OnRotationCenterChanged((RotationCenter)e.NewValue);
        }



        public TankInstance AlternativeTankInstance
        {
            get { return (TankInstance)GetValue(AlternativeTankInstanceProperty); }
            set { SetValue(AlternativeTankInstanceProperty, value); }
        }

        public static readonly DependencyProperty AlternativeTankInstanceProperty =
            DependencyProperty.Register("AlternativeTankInstance", typeof(TankInstance), typeof(ModelView), new PropertyMetadata(null, ModelView.OnAlternativeTankInstanceChanged));

        private static void OnAlternativeTankInstanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModelView)d).OnAlternativeTankInstanceChanged((TankInstance)e.NewValue);
        }



        public HangarSceneSource HangarSceneSource
        {
            get { return (HangarSceneSource)GetValue(HangarSceneSourceProperty); }
            set { SetValue(HangarSceneSourceProperty, value); }
        }

        public static readonly DependencyProperty HangarSceneSourceProperty =
            DependencyProperty.Register("HangarSceneSource", typeof(HangarSceneSource), typeof(ModelView), new PropertyMetadata(HangarSceneSource.Primary, ModelView.OnHangarSceneSourceChanged));

        private static void OnHangarSceneSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModelView)d).OnHangarSceneSourceChanged((HangarSceneSource)e.NewValue);
        }

        private void OnHangarSceneSourceChanged(HangarSceneSource hangarSceneSource)
        {
            if (hangarSceneSource == HangarSceneSource.Alternative)
            {
                if (_alternativeDXElement == null)
                    this.InitializeAlternativeView();

                _alternativeDXElement.Visibility = Visibility.Visible;
                this.DXElement.Visibility = Visibility.Hidden;
            }
            else
            {
                if (_alternativeDXElement != null)
                {
                    if (this.AlternativeTankInstance == null)
                        this.RemoveAlternativeView();
                    else
                        _alternativeDXElement.Visibility = Visibility.Hidden;
                }

                this.DXElement.Visibility = Visibility.Visible;
            }

        }

        private void OnAlternativeTankInstanceChanged(TankInstance tankInstance)
        {
            if(tankInstance == null && _alternativeDXElement !=null)
            {
                this.RemoveAlternativeView();
                return;
            }

            if (_alternativeDXElement == null)
                this.InitializeAlternativeView();

            _alternativeHangarScene.TankModel = new TankModel(tankInstance);
        }

        private void OnRotationCenterChanged(RotationCenter rotationCenter)
        {
            this.ConfigHangarScene(h => h.RotationCenter = rotationCenter);
        }

        private void OnFileSourceChanged(FileSource fileSource)
        {
            this.ConfigHangarScene(h => h.FileSource = fileSource);
        }

        private void OnShowCamouflageChanged(bool show)
        {
            this.ConfigHangarScene(h => h.ShowCamouflage = show);
        }

        private void OnFovChanged(double fov)
        {
            this.ConfigHangarScene(h => h.Fov = fov);
        }

        private void OnProjectionModeChanged(ProjectionMode projectionMode)
        {
            this.ConfigHangarScene(h => h.ProjectionMode = projectionMode);
        }

        private void OnCollisionModelRenderParametersChanged(CollisionModelRenderParameters collisionModelRenderParameters)
        {
            this.ConfigHangarScene(h => h.CollisionModelRenderParameters = collisionModelRenderParameters);
        }

        private void OnShowGunChanged(bool show)
        {
            this.ConfigHangarScene(h => h.ShowGun = show);
        }

        private void OnShowTurretChanged(bool show)
        {
            this.ConfigHangarScene(h => h.ShowTurret = show);
        }

        private void OnShowHullChanged(bool show)
        {
            this.ConfigHangarScene(h => h.ShowHull = show);
        }

        private void OnShowChassisChanged(bool show)
        {
            this.ConfigHangarScene(h => h.ShowChassis = show);
        }

        private void OnGraphicsSettingsChanged(GraphicsSettings graphicsSettings)
        {
            this.ConfigHangarScene(h => h.GraphicsSettings = graphicsSettings);
        }

        private void OnLookDirectionChanged(Vector3D vector3D)
        {
            this.ConfigHangarScene(h => h.LookDirection = new Vector3((float)vector3D.X, (float)vector3D.Y, (float)vector3D.Z));
        }

        private void OnCameraTransformChanged(Transform3D transform)
        {
            var matrix = transform.Value;
            var hangarSceneTransform = new Matrix(
                    (float)matrix.M11, (float)matrix.M12, (float)matrix.M13, (float)matrix.M14,
                    (float)matrix.M21, (float)matrix.M22, (float)matrix.M23, (float)matrix.M24,
                    (float)matrix.M31, (float)matrix.M32, (float)matrix.M33, (float)matrix.M34,
                    (float)matrix.OffsetX, (float)matrix.OffsetY, (float)matrix.OffsetZ, (float)matrix.M44);

            this.ConfigHangarScene(h => h.Transform = hangarSceneTransform);
        }

        private void OnZoomChanged(double zoom)
        {
            this.ConfigHangarScene(h => h.Zoom = zoom);
        }

        private void OnRenderActivityLevelChanged(RenderActivityLevel renderActivityLevel)
        {
            this.DXElement.RenderActivityLevel = renderActivityLevel;
        }

        private void OnTankInstanceChanged(TankInstance tankInstance)
        {
            this.HangarScene.TankModel = new TankModel(tankInstance);
        }

        private void OnModelTypeChanged(ModelType modelType)
        {
            this.ConfigHangarScene(h => h.ModelType = modelType);
        }

    }
}

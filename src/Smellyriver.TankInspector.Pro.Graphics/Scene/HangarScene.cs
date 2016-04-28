using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using SharpDX;
using SharpDX.Direct3D9;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Graphics.Frameworks;
using Smellyriver.TankInspector.Pro.Graphics.Smaa;
using Smellyriver.TankInspector.Pro.IO;
using Smellyriver.TankInspector.Pro.Modularity.Tasks;
using Smellyriver.TankInspector.Pro.Repository;
using Color = SharpDX.Color;
using D3DEffect = SharpDX.Direct3D9.Effect;
using Matrix = SharpDX.Matrix;
using Point = System.Windows.Point;
using WpfColor = System.Windows.Media.Color;

namespace Smellyriver.TankInspector.Pro.Graphics.Scene
{
    partial class HangarScene : SceneBase<D3D9>, ISnapshotProvider
    {
        private static HangarScene _current;
        public static HangarScene Current
        {
            get { return _current; }
        }

        public TankModel TankModel
        {
            get { return (TankModel)GetValue(TankModelProperty); }
            set { SetValue(TankModelProperty, value); }
        }


        public static readonly DependencyProperty TankModelProperty =
            DependencyProperty.Register("TankModel", typeof(TankModel), typeof(HangarScene), new PropertyMetadata(null, HangarScene.OnTankModelChanged));

        private static void OnTankModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HangarScene)d).OnTankModelChanged((TankModel)e.OldValue, (TankModel)e.NewValue);
        }


        public GraphicsSettings GraphicsSettings { get; set; }

        private ModuleModelRenderer _hullModelRenderer;
        private ModuleModelRenderer _turretModelRenderer;
        private ModuleModelRenderer _chassisModelRenderer;
        private ModuleModelRenderer _gunModelRenderer;

        public bool ShowHull { get; set; }
        public bool ShowTurret { get; set; }
        public bool ShowGun { get; set; }
        public bool ShowChassis { get; set; }


        public HangarScene()
        {
            _current = this;
            this.GraphicsSettings = GraphicsSettings.Default;

            _armorColorMapDirty = true;
            _fps = new FPS();

            _hullModelRenderer = new ModuleModelRenderer(this);
            _turretModelRenderer = new ModuleModelRenderer(this);
            _chassisModelRenderer = new ModuleModelRenderer(this);
            _gunModelRenderer = new ModuleModelRenderer(this);

        }


        private void OnTankModelChanged(TankModel oldValue, TankModel newValue)
        {
            if (oldValue != null)
            {
                oldValue.HullChanged -= TankModel_HullChanged;
                oldValue.TurretChanged -= TankModel_TurretChanged;
                oldValue.GunChanged -= TankModel_GunChanged;
                oldValue.ChassisChanged -= TankModel_ChassisChanged;
            }

            if (newValue != null)
            {

                newValue.HullChanged += TankModel_HullChanged;
                newValue.TurretChanged += TankModel_TurretChanged;
                newValue.GunChanged += TankModel_GunChanged;
                newValue.ChassisChanged += TankModel_ChassisChanged;

                this.LoadChassisModel();
                this.LoadHullModel();
                this.LoadTurretModel();
                this.LoadGunModel();
                this.LoadCamouflageExclusionMask();

                this.InitializeFileLocator();
            }
            else
            {
                //todo: clear
            }
        }


        void TankModel_ChassisChanged(object sender, EventArgs e)
        {
            this.LoadChassisModel();
        }

        void TankModel_GunChanged(object sender, EventArgs e)
        {
            this.LoadGunModel();
        }

        void TankModel_TurretChanged(object sender, EventArgs e)
        {
            this.LoadTurretModel();
        }

        void TankModel_HullChanged(object sender, EventArgs e)
        {
            this.LoadHullModel();
        }

        private void LoadHullModel()
        {
            _hullModelRenderer.Model = this.TankModel.HullModel;
        }

        private void LoadChassisModel()
        {
            _chassisModelRenderer.Model = this.TankModel.ChassisModel;
        }

        private void LoadGunModel()
        {
            _gunModelRenderer.Model = this.TankModel.GunModel;
        }

        private void LoadTurretModel()
        {
            _turretModelRenderer.Model = this.TankModel.TurretModel;
        }

        private void ClearTracerFrom(ModuleMesh mesh)
        {
            Monitor.Enter(_projectileTracer);
            try
            {
                _projectileTracer.RemoveAll(t => t.Hit(mesh));
            }
            finally
            {
                Monitor.Exit(_projectileTracer);
            }
        }

        public Matrix Transform
        {
            get { return (Matrix)GetValue(TransformProperty); }
            set { SetValue(TransformProperty, value); }
        }

        public static readonly DependencyProperty TransformProperty =
            DependencyProperty.Register("Transform", typeof(Matrix), typeof(HangarScene), new PropertyMetadata(Matrix.Identity));

        private ProjectionMode _projectionMode;
        public ProjectionMode ProjectionMode
        {
            get { return _projectionMode; }
            set
            {
                _projectionMode = value;
                this.SetViewProj(_width, _height);
            }
        }

        private double _fov;

        public double Fov
        {
            get { return _fov; }
            set
            {
                _fov = value;
                this.SetViewProj(_width, _height);
            }
        }

        private bool _showCamouflage;

        public bool ShowCamouflage
        {
            get { return _showCamouflage; }
            set
            {
                _showCamouflage = value;
            }
        }

        private void LoadCamouflageExclusionMask()
        {
            if (_camouflageExclusionMask == null)
            {
                var texturePath = TankModel.Camouflage.ExclusionMask;
                if (string.IsNullOrEmpty(texturePath))
                    return;

                try
                {
                    using (var Stream = Utility.OpenTexture(TankModel.GameClient.PackageIndexer, texturePath))
                    {
                        _camouflageExclusionMask = Texture.FromStream(Renderer.Device, Stream);

                    }

                    using (var Stream = Utility.OpenTexture(TankModel.GameClient.PackageIndexer, TankModel.Camouflage.Texture))
                    {
                        _camouflageMap = Texture.FromStream(Renderer.Device, Stream);
                    }
                }
                catch (Exception)
                {
                    this.LogInfo("can't load texture {0} for {1}", texturePath, "camouflage exclusion mask");
                }
            }
        }

        private FileSource? _fileSource;

        public FileSource? FileSource
        {
            get { return _fileSource; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                if (_fileSource == value)
                    return;

                _fileSource = value;

                if (this.TankModel != null)
                    this.InitializeFileLocator();
            }
        }

        private void InitializeFileLocator()
        {
            var fileLocator = new LocalGameClientFileLocator((LocalGameClient)this.TankModel.TankInstance.Repository,
                                                              this.FileSource.Value);

            _gunModelRenderer.FileLocator = fileLocator;
            _turretModelRenderer.FileLocator = fileLocator;
            _chassisModelRenderer.FileLocator = fileLocator;
            _hullModelRenderer.FileLocator = fileLocator;
        }


        private CollisionModelRenderParameters _collisionModelRenderParameters;
        public CollisionModelRenderParameters CollisionModelRenderParameters
        {
            get { return _collisionModelRenderParameters; }
            set
            {
                if (_collisionModelRenderParameters != null)
                    _collisionModelRenderParameters.ArmorColorMapInvalidated -= CollisionModelRenderParameters_ArmorColorMapInvalidated;

                _collisionModelRenderParameters = value;

                if (_collisionModelRenderParameters != null)
                    _collisionModelRenderParameters.ArmorColorMapInvalidated += CollisionModelRenderParameters_ArmorColorMapInvalidated;

            }
        }

        void CollisionModelRenderParameters_ArmorColorMapInvalidated(object sender, EventArgs e)
        {
            this.UpdateArmorColorMap();
        }


        private ModelType _modelType;

        public ModelType ModelType
        {
            get { return _modelType; }
            set
            {
                _modelType = value;
                _gunModelRenderer.ModelType = _modelType;
                _turretModelRenderer.ModelType = _modelType;
                _chassisModelRenderer.ModelType = _modelType;
                _hullModelRenderer.ModelType = _modelType;
            }
        }



        public bool IsHitTestEnabled
        {
            get { return (bool)GetValue(IsHitTestEnabledProperty); }
            set { SetValue(IsHitTestEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsHitTestEnabledProperty =
            DependencyProperty.Register("IsHitTestEnabled", typeof(bool), typeof(HangarScene), new PropertyMetadata(true));

        public Point MousePoition
        {
            get { return (Point)GetValue(MousePoitionProperty); }
            set { SetValue(MousePoitionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MousePoition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MousePoitionProperty =
            DependencyProperty.Register("MousePoition", typeof(Point), typeof(HangarScene), new PropertyMetadata(new Point(),
                (d, e) => ((HangarScene)d).OnMousePoitionChanged((Point)e.OldValue, (Point)e.NewValue)));

        private void OnMousePoitionChanged(Point oldPoint, Point newPoint)
        {
            if (this.IsHitTestEnabled)
            {
                var x = (float)MousePoition.X;
                var y = (float)MousePoition.Y;

                var ray = GetPickRay(x, y);
                var invWorldTrans = Matrix.Invert(Transform);
                ray.Position = invWorldTrans.TransformCoord(ray.Position);
                ray.Direction = invWorldTrans.TransformNormal(ray.Direction);
                ray.Direction.Normalize();

                if (!TryPickTracer(ray))
                    TryHitTestCollisionModel(ray);
            }
        }


        public int TriangleCount
        {
            get { return (int)GetValue(TriangleCountProperty); }
            set { SetValue(TriangleCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TriangleCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TriangleCountProperty =
            DependencyProperty.Register("TriangleCount", typeof(int), typeof(HangarScene), new PropertyMetadata(0));

        public ShootTestResult ShootTestResult
        {
            get { return (ShootTestResult)GetValue(ShootTestResultProperty); }
            set { SetValue(ShootTestResultProperty, value); }
        }

        public static readonly DependencyProperty ShootTestResultProperty =
            DependencyProperty.Register("ShootTestResult", typeof(ShootTestResult), typeof(HangarScene), new PropertyMetadata(null));



        public TestShellInfo TestShell
        {
            get { return (TestShellInfo)GetValue(TestShellProperty); }
            set { SetValue(TestShellProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TestShell.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TestShellProperty =
            DependencyProperty.Register("TestShell", typeof(TestShellInfo), typeof(HangarScene), new PropertyMetadata(new TestShellInfo(ShellType.AP, 0.0),
                (d, e) => ((HangarScene)d).OnTestShellChanged((TestShellInfo)e.OldValue, (TestShellInfo)e.NewValue)));

        private void OnTestShellChanged(TestShellInfo oldValue, TestShellInfo newValue)
        {

            Monitor.Enter(_projectileTracer);
            try
            {
                foreach (var tracer in _projectileTracer)
                {
                    tracer.Refresh(newValue);
                }
            }
            finally
            {
                Monitor.Exit(_projectileTracer);
            }

        }

        public bool IsMouseOverModel
        {
            get { return (bool)GetValue(IsMouseOverModelProperty); }
            set { SetValue(IsMouseOverModelProperty, value); }
        }

        public static readonly DependencyProperty IsMouseOverModelProperty =
            DependencyProperty.Register("IsMouseOverModel", typeof(bool), typeof(HangarScene), new PropertyMetadata(false));

        public double FPS
        {
            get { return (double)GetValue(FPSProperty); }
            set { SetValue(FPSProperty, value); }
        }

        public static readonly DependencyProperty FPSProperty =
            DependencyProperty.Register("FPS", typeof(double), typeof(HangarScene), new PropertyMetadata(0.0));



        public CameraMode CameraMode
        {
            get { return (CameraMode)GetValue(CameraModeProperty); }
            set { SetValue(CameraModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CameraMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CameraModeProperty =
            DependencyProperty.Register("CameraMode", typeof(CameraMode), typeof(HangarScene), new PropertyMetadata(CameraMode.TrackBall,
                (d, e) => ((HangarScene)d).OnCameraModeChanged((CameraMode)e.OldValue, (CameraMode)e.NewValue)));



        public double ShotDistance
        {
            get { return (double)GetValue(ShotDistanceProperty); }
            set { SetValue(ShotDistanceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShotDistance.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShotDistanceProperty =
            DependencyProperty.Register("ShotDistance", typeof(double), typeof(HangarScene), new PropertyMetadata(100.0,
                (d, e) => ((HangarScene)d).OnShotDistanceChanged((double)e.OldValue, (double)e.NewValue)));

        private void OnShotDistanceChanged(double oldValue, double newValue)
        {
            _shotPosition.Z = (float)-newValue;
            UpdateView();
        }

        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomTransform.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomProperty =
            DependencyProperty.Register("Zoom", typeof(double), typeof(HangarScene), new PropertyMetadata(0.0,
                (d, e) => ((HangarScene)d).OnZoomTransformChanged((double)e.OldValue, (double)e.NewValue)));

        private void OnZoomTransformChanged(double oldZoom, double newZoom)
        {
            if (this.ProjectionMode == ProjectionMode.Orthographic)
                this.SetViewProj(_width, _height);
            else
                this.UpdateView();
        }



        public RotationCenter RotationCenter
        {
            get { return (RotationCenter)GetValue(RotationCenterProperty); }
            set { SetValue(RotationCenterProperty, value); }
        }

        public static readonly DependencyProperty RotationCenterProperty =
            DependencyProperty.Register("RotationCenter", typeof(RotationCenter), typeof(HangarScene), new PropertyMetadata(RotationCenter.Hull));


        private void UpdateView()
        {
            _view = Matrix.LookAtLH(_cameraPosition, _targetPosition, Vector3.UnitY);
            _invView = Matrix.Invert(_view);
            _viewProj = Matrix.Multiply(_view, _proj);
        }

        public Vector3 LookDirection
        {
            get { return (Vector3)GetValue(LookDirectionProperty); }
            set { SetValue(LookDirectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LookDirection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LookDirectionProperty =
            DependencyProperty.Register("LookDirection", typeof(Vector3), typeof(HangarScene), new PropertyMetadata(Vector3.ForwardLH,
                (d, e) => ((HangarScene)d).OnLookDirectionChanged((Vector3)e.OldValue, (Vector3)e.NewValue)));

        private void OnLookDirectionChanged(Vector3 oldDirection, Vector3 newDirection)
        {
            var shot = Matrix.LookAtLH(_shotPosition, _targetPosition, Vector3.UnitY);
            var invShot = Matrix.Invert(shot);
            var newTarget = shot.TransformCoord(_targetPosition);
            newTarget.X += newDirection.X;
            newTarget.Y -= newDirection.Y;
            newTarget = invShot.TransformCoord(newTarget);

            var targetDir = newTarget - _shotPosition;
            targetDir.Normalize();
            newTarget = _shotPosition + targetDir * (_targetPosition - _shotPosition).Length();

            //to do new target in tank~
            _targetPosition = newTarget;
            UpdateView();
        }

        private void OnCameraModeChanged(CameraMode oldMode, CameraMode newMode)
        {
            const float _animationSpeed = 25.0f;
            if (newMode == CameraMode.TrackBall)
            {
                if (_targetPosition != Vector3.Zero)
                {
                    _animationDirection = Vector3.Normalize(_targetPosition) * _animationSpeed;
                    _remainingTime = _targetPosition.Length() / _animationSpeed;
                }
            }
        }



        public bool IsInitializing
        {
            get { return (bool)GetValue(IsInitializingPropertyKey.DependencyProperty); }
            private set { SetValue(IsInitializingPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey IsInitializingPropertyKey =
            DependencyProperty.RegisterReadOnly("IsInitializing", typeof(bool), typeof(HangarScene), new PropertyMetadata(true));

        public static readonly DependencyProperty IsInitializingProperty = IsInitializingPropertyKey.DependencyProperty;


        const double _maxHighlightDelta = 0.35;
        const double _maxLowlightDelta = 0.25;

        private double _highLightValue;


        private Matrix _proj;
        private Matrix _view;
        private Matrix _invView;
        private Matrix _viewProj;

        private Surface _positionSurface;
        private Surface _normalSurface;
        private Surface _colorSurface;
        private Surface _finalColorSurface;
        private Surface _depthStencil;

        private Texture _camouflageExclusionMask;
        private Texture _camouflageMap;
        private Texture _normalMap;
        private Texture _positionMap;
        private Texture _colorMap;
        private Texture _finalColorMap;
        private Texture _randomMap;
        private Texture _armorColorMap;
        private bool _armorColorMapDirty;

        private QuadRender _quadRender;

        private Effect _clearGBufferEffect;
        private Effect _renderGBufferEffect;
        private Effect _showGBufferEffect;
        private Effect _lightingTankEffect;
        private Effect _buildArmorColorEffect;

        private SMAA _smaa;
        private int _width;
        private int _height;
        private FPS _fps;

        private CollisionModelHitTestResult _lastHitTestResult;

        private List<ProjectileTracer> _projectileTracer = new List<ProjectileTracer>();

        private Vector3 _targetPosition = Vector3.Zero;
        private Vector3 _animationDirection = Vector3.Zero;
        private double _remainingTime = 0;

        private Vector3 _shotPosition = new Vector3(0, 0, -100);

        private Vector3 _cameraPosition
        {
            get
            {
                var dir = _shotPosition - _targetPosition;
                dir.Normalize();
                return _targetPosition + dir * 14.0f / (float)Zoom * 39.6f / (float)this.Fov;
            }
        }

        private int _sceneWidth = 1;
        private int _sceneHeight = 1;

        private void UpdateTargetPositionAnimation(double deltaTime)
        {
            if (_remainingTime > 0)
            {
                _targetPosition -= _animationDirection * (float)deltaTime;
                _remainingTime -= deltaTime;

                if (_remainingTime < 0)
                {
                    _targetPosition = Vector3.Zero;
                }

                UpdateView();
            }
        }

        private void SetViewProj(int width, int height)
        {
            _view = Matrix.LookAtLH(_cameraPosition, _targetPosition, Vector3.UnitY);
            _invView = Matrix.Invert(_view);

            if (this.ProjectionMode == ProjectionMode.Perspective)
                _proj = Matrix.PerspectiveFovLH((float)(this.Fov * Math.PI / 180), (float)width / (float)height, 0.1f, 707.0f);
            else
                _proj = Matrix.OrthoLH((float)width / 100 / (float)this.Zoom, (float)height / 100 / (float)this.Zoom, 0.1f, 707.0f);

            _viewProj = Matrix.Multiply(_view, _proj);
        }

        void OnRenderResetted(object sender, DrawEventArgs e)
        {
            int w = (int)Math.Ceiling(e.RenderSize.Width);
            int h = (int)Math.Ceiling(e.RenderSize.Height);
            ResetScene(w, h);
        }

        private void ResetScene(int width, int height)
        {
            if (this.IsInitializing)
            {
                _sceneWidth = width;
                _sceneHeight = height;
                return;
            }

            this.LogInfo("hangar scene reset to ({0},{1}) ", width, height);

            var device = Renderer.Device;
            _width = width;
            _height = height;
            SetViewProj(width, height);

            Disposer.RemoveAndDispose(ref _depthStencil);
            Disposer.RemoveAndDispose(ref _quadRender);
            Disposer.RemoveAndDispose(ref _finalColorSurface);
            Disposer.RemoveAndDispose(ref _colorSurface);
            Disposer.RemoveAndDispose(ref _positionSurface);
            Disposer.RemoveAndDispose(ref _normalSurface);
            Disposer.RemoveAndDispose(ref _finalColorMap);
            Disposer.RemoveAndDispose(ref _colorMap);
            Disposer.RemoveAndDispose(ref _positionMap);
            Disposer.RemoveAndDispose(ref _normalMap);

            _normalMap = new Texture(device, width, height, 1, Usage.RenderTarget, Format.A16B16G16R16F, Pool.Default);
            _positionMap = new Texture(device, width, height, 1, Usage.RenderTarget, Format.A16B16G16R16F, Pool.Default);
            _colorMap = new Texture(device, width, height, 1, Usage.RenderTarget, Format.A16B16G16R16F, Pool.Default);
            _finalColorMap = new Texture(device, width, height, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
            _normalSurface = _normalMap.GetSurfaceLevel(0);
            _positionSurface = _positionMap.GetSurfaceLevel(0);
            _colorSurface = _colorMap.GetSurfaceLevel(0);
            _finalColorSurface = _finalColorMap.GetSurfaceLevel(0);
            _quadRender = new QuadRender(device, width, height);

            _smaa.Reset(width, height, _quadRender);

            ResetHUD(width, height);

            _depthStencil = Surface.CreateDepthStencil(device, width, height, Format.D24S8, MultisampleType.None, 0, true);

            device.DepthStencilSurface = _depthStencil;
        }

        public BitmapSource[] YawAnimationSnapshot(Rect rect,
                                                   double sampleRatio,
                                                   WpfColor? backgroundColor,
                                                   double rotationSpeed,
                                                   double frameRate,
                                                   IProgressScope progress,
                                                   Func<bool> getIsCancelled)
        {
            var oldTransform = this.Transform;

            try
            {
                var frameTime = 1.0 / frameRate;
                var frameRotation = rotationSpeed * frameTime;
                var totalFrames = (int)Math.Round(360.0 / frameRotation);
                frameRotation = 360.0 / totalFrames;

                var frames = new BitmapSource[totalFrames];

                using (var context = new SnapshotContext(this.Renderer.Device, _view, rect, sampleRatio))
                {
                    for (var i = 0; i < totalFrames; ++i)
                    {
                        if (getIsCancelled())
                            return null;

                        var yaw = (double)i * frameRotation;
                        this.Transform = Matrix.Identity
                                         * Matrix.RotationY((float)DXUtils.ConvertDegreesToRadians(yaw))
                                         * oldTransform;

                        frames[i] = this.Snapshot(context, backgroundColor);
                        progress.ReportProgress((double)i / totalFrames);

                    }
                }

                if (getIsCancelled())
                    return null;

                return frames;
            }
            finally
            {
                this.Transform = oldTransform;
            }
        }


        private BitmapSource Snapshot(SnapshotContext context, WpfColor? backgroundColor = null)
        {
            var triangleCount = 0;

            var device = this.Renderer.Device;
            device.BeginScene();

            var backupSurface = device.GetRenderTarget(0);

            device.SetRenderTarget(0, context.ColorSurface);
            device.SetRenderTarget(1, context.NormalSurface);
            device.SetRenderTarget(2, context.PositionSurface);


            var clearColor = backgroundColor == null
                           ? new Color(0x80, 0x80, 0x80, 0)
                           : new Color(backgroundColor.Value.R,
                                               backgroundColor.Value.G,
                                               backgroundColor.Value.B,
                                               backgroundColor.Value.A);

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer | ClearFlags.Stencil, clearColor, 1.0f, 0);

            _clearGBufferEffect.Technique = _clearGBufferEffect.GetTechnique(0);
            _clearGBufferEffect.Begin();
            _clearGBufferEffect.BeginPass(0);

            context.QuadRender.Render(Renderer.Device);

            _clearGBufferEffect.EndPass();
            _clearGBufferEffect.End();

            if (ModelType == ModelType.Collision)
            {
                device.SetRenderState(RenderState.CullMode, Cull.None);
            }


            _renderGBufferEffect.Technique = _renderGBufferEffect.GetTechnique(0);
            _renderGBufferEffect.SetValue("viewProj", context.ViewProjection);
            _renderGBufferEffect.SetValue("world", Transform);
            _renderGBufferEffect.SetValue("maxAnisotropy", 16);

            if (ModelType == ModelType.Collision)
            {
                _renderGBufferEffect.SetValue("useArmor", true);
            }
            else
            {
                _renderGBufferEffect.SetValue("useArmor", false);
            }

            this.RenderModels(ref triangleCount);


            if (ModelType == ModelType.Collision)
            {
                device.SetRenderState(RenderState.CullMode, Cull.Counterclockwise);
            }

            device.SetRenderTarget(0, context.FinalColorSurface);
            device.SetRenderTarget(1, null);
            device.SetRenderTarget(2, null);

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, clearColor, 1.0f, 0);

            device.SetTexture(0, context.ColorMap);
            device.SetTexture(1, context.NormalMap);
            device.SetTexture(2, context.PositionMap);
            device.SetTexture(3, _randomMap);

            if (ModelType == ModelType.Collision)
            {
                var settings = this.CollisionModelRenderParameters;
                device.SetTexture(4, _armorColorMap);

                _lightingTankEffect.SetValue("tankThickestArmor", (float)settings.TankThickestArmor + 0.125f);
                _lightingTankEffect.SetValue("tankThinnestArmor", (float)settings.TankThinnestArmor - 0.125f);
                _lightingTankEffect.SetValue("tankThickestSpacingArmor", (float)settings.TankThickestSpacingArmor + 0.125f);
                _lightingTankEffect.SetValue("tankThinnestSpacingArmor", (float)settings.TankThinnestSpacingArmor - 0.125f);

                var regularArmorValueSelectionMax = (float)Math.Max(settings.RegularArmorValueSelectionBegin, settings.RegularArmorValueSelectionEnd);
                var regularArmorValueSelectionMin = (float)Math.Min(settings.RegularArmorValueSelectionBegin, settings.RegularArmorValueSelectionEnd);

                _lightingTankEffect.SetValue("regularArmorValueSelectionMax", regularArmorValueSelectionMax + 0.4f);
                _lightingTankEffect.SetValue("regularArmorValueSelectionMin", regularArmorValueSelectionMin - 0.4f);

                var spacingArmorValueSelectionMax = (float)Math.Max(settings.SpacingArmorValueSelectionBegin, settings.SpacingArmorValueSelectionEnd);
                var spacingArmorValueSelectionMin = (float)Math.Min(settings.SpacingArmorValueSelectionBegin, settings.SpacingArmorValueSelectionEnd);

                _lightingTankEffect.SetValue("spacingArmorValueSelectionMax", spacingArmorValueSelectionMax + 0.4f);
                _lightingTankEffect.SetValue("spacingArmorValueSelectionMin", spacingArmorValueSelectionMin - 0.4f);

                _lightingTankEffect.SetValue("hasRegularArmorHintValue", false);
                _lightingTankEffect.SetValue("hasSpacingArmorHintValue", false);
                _lightingTankEffect.SetValue("useBlackEdge", GraphicsSettings.CollisionModelStrokeEnabled);

                _lightingTankEffect.Technique = _lightingTankEffect.GetTechnique(1);
            }
            else
            {
                _lightingTankEffect.Technique = _lightingTankEffect.GetTechnique(0);
            }

            _lightingTankEffect.SetValue("useSSAO", GraphicsSettings.SSAOEnabled);

            _lightingTankEffect.Begin();
            _lightingTankEffect.BeginPass(0);

            context.QuadRender.Render(device);

            _lightingTankEffect.EndPass();
            _lightingTankEffect.End();

            device.SetRenderTarget(0, context.DstSurface);
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer | ClearFlags.Stencil, new Color(0x80, 0x80, 0x80, 0), 1.0f, 0);
            context.SMAA.Apply(context.FinalColorMap, context.FinalColorMap, context.DstSurface);

            device.SetRenderTarget(0, backupSurface);

            device.EndScene();
            device.Present();


            var shootMapSurface = context.ShootMap.GetSurfaceLevel(0);

            device.GetRenderTargetData(context.DstSurface, shootMapSurface);

            DataStream data;

            context.ShootMap.LockRectangle(0, LockFlags.ReadOnly, out data);

            var bufferSize = context.Width * context.Height * 4;
            var buffer = new byte[bufferSize];
            data.Read(buffer, 0, bufferSize);
            var bitmap = BitmapSource.Create(context.Width, context.Height, 96, 96, PixelFormats.Bgra32, null, buffer, context.Width * 4);

            context.ShootMap.UnlockRectangle(0);

            device.DepthStencilSurface = _depthStencil;

            return bitmap;

        }

        private void RenderModels(ref int triangleCount)
        {
            if (this.ShowTurret)
            {
                var rotation = Matrix.RotationY((float)DXUtils.ConvertDegreesToRadians(this.TankModel.TankInstance.Transform.TurretYaw));

                if (this.RotationCenter == RotationCenter.Hull)
                    _turretModelRenderer.Transform = rotation 
                                                   * Matrix.Translation(this.TankModel.TurretPosition);
                else
                    _turretModelRenderer.Transform = rotation;

                _renderGBufferEffect.SetValue("world", _turretModelRenderer.Transform * Transform);

                _turretModelRenderer.Render(_renderGBufferEffect, ref triangleCount);
            }

            if (this.ShowGun)
            {
                _gunModelRenderer.Transform = Matrix.RotationX((float)DXUtils.ConvertDegreesToRadians(this.TankModel.TankInstance.Transform.GunPitch))
                                            * Matrix.Translation(this.TankModel.GunPosition)
                                            * _turretModelRenderer.Transform;
                _renderGBufferEffect.SetValue("world", _gunModelRenderer.Transform * Transform);

                _gunModelRenderer.Render(_renderGBufferEffect, ref triangleCount);
            }

            if (this.ShowHull)
            {
                if (this.RotationCenter == RotationCenter.Hull)
                    _hullModelRenderer.Transform = Matrix.Identity;
                else
                    _hullModelRenderer.Transform = Matrix.Translation(-this.TankModel.TurretPosition);

                _renderGBufferEffect.SetValue("world", _hullModelRenderer.Transform * Transform);

                _hullModelRenderer.Render(_renderGBufferEffect, ref triangleCount);
            }

            if (this.ShowChassis)
            {
                _renderGBufferEffect.SetValue("showCamouflage", false);

                _chassisModelRenderer.Transform = _hullModelRenderer.Transform * Matrix.Translation(-this.TankModel.HullPosition);
                _renderGBufferEffect.SetValue("world", _chassisModelRenderer.Transform * Transform);

                _chassisModelRenderer.Render(_renderGBufferEffect, ref triangleCount);
            }

        }

        public BitmapSource Snapshot(Rect rect, double sampleRatio, WpfColor? backgroundColor = null)
        {
            using (var context = new SnapshotContext(this.Renderer.Device, _view, rect, sampleRatio))
            {
                return this.Snapshot(context, backgroundColor);
            }
        }

        private void RenderGBuffer()
        {
            SetGBuffer();
            ClearGBuffer();
            var device = Renderer.Device;

            if (ModelType == ModelType.Collision)
            {
                device.SetRenderState(RenderState.CullMode, Cull.None);
            }

            if (GraphicsSettings.WireframeMode)
            {

                device.SetRenderState(RenderState.FillMode, FillMode.Wireframe);
                RenderScene();
                device.SetRenderState(RenderState.FillMode, FillMode.Solid);
            }
            else
            {
                RenderScene();
            }

            if (ModelType == ModelType.Collision)
            {
                device.SetRenderState(RenderState.CullMode, Cull.Counterclockwise);
            }

            ResolveGBuffer();
        }

        private void RenderScene()
        {
            if (this.TankModel == null)
                return;

            var device = Renderer.Device;

            int triangleCount = 0;

            _renderGBufferEffect.Technique = _renderGBufferEffect.GetTechnique(0);
            _renderGBufferEffect.SetValue("viewProj", _viewProj);

            _renderGBufferEffect.SetValue("world", Transform);

            if (ModelType == ModelType.Collision)
            {
                _renderGBufferEffect.SetValue("useArmor", true);
                _renderGBufferEffect.SetValue("showCamouflage", false);
            }
            else
            {
                if (ShowCamouflage)
                {
                    _renderGBufferEffect.SetValue("showCamouflage", ShowCamouflage);
                    device.SetTexture(4, _camouflageExclusionMask);
                    device.SetTexture(5, _camouflageMap);
                }
                else
                {
                    _renderGBufferEffect.SetValue("showCamouflage", false);
                }
                _renderGBufferEffect.SetValue("useArmor", false);
            }

            _renderGBufferEffect.Begin();
            _renderGBufferEffect.BeginPass(1);

            if (Monitor.TryEnter(_projectileTracer))
            {
                try
                {
                    foreach (var tracer in _projectileTracer)
                    {
                        tracer.Render();
                    }
                }
                finally
                {
                    Monitor.Exit(_projectileTracer);
                }
            }
            _renderGBufferEffect.EndPass();
            _renderGBufferEffect.End();

            _renderGBufferEffect.SetValue("maxAnisotropy", GraphicsSettings.AnisotropicFilterLevel);

            this.RenderModels(ref triangleCount);

            this.TriangleCount = triangleCount;
        }




        private void ClearGBuffer()
        {
            _clearGBufferEffect.Technique = _clearGBufferEffect.GetTechnique(0);
            _clearGBufferEffect.Begin();
            _clearGBufferEffect.BeginPass(0);
            _quadRender.Render(Renderer.Device);
            _clearGBufferEffect.EndPass();
            _clearGBufferEffect.End();
        }

        private void ResolveGBuffer()
        {
            var device = Renderer.Device;
            //device.SetRenderTarget(0, null);
            device.SetRenderTarget(1, null);
            device.SetRenderTarget(2, null);
        }

        private void SetGBuffer()
        {
            var device = Renderer.Device;
            device.SetRenderTarget(0, _colorSurface);
            device.SetRenderTarget(1, _normalSurface);
            device.SetRenderTarget(2, _positionSurface);
        }


        protected override void Attach()
        {
            if (this.Renderer == null)
                return;

            this.IsInitializing = true;

            this.Renderer.Resetted += OnRenderResetted;
            var device = Renderer.Device;

            var dispatcher = Dispatcher.CurrentDispatcher;

            Task.Factory.StartNew(() =>
                {

                    // Compiles the effect
                    _showGBufferEffect = D3DEffect.FromFile(device, @"Graphics\Effect\ShowGBuffer.fx", ShaderFlags.None);

                    _clearGBufferEffect = D3DEffect.FromFile(device, @"Graphics\Effect\ClearGBuffer.fx", ShaderFlags.None);

                    _renderGBufferEffect = D3DEffect.FromFile(device, @"Graphics\Effect\RenderGBuffer.fx", ShaderFlags.None);

                    _lightingTankEffect = D3DEffect.FromFile(device, @"Graphics\Effect\LightingTank.fx", ShaderFlags.None);

                    _buildArmorColorEffect = D3DEffect.FromFile(device, @"Graphics\Effect\BuildArmorColor.fx", ShaderFlags.None);

                    _randomMap = Texture.FromFile(device, @"Graphics\Texture\random.dds");

                    _smaa = new SMAA(device, 1, 1, SMAA.Preset.PRESET_ULTRA);

                    this.LogInfo("hangar scene attached to device");

                    InitializeHUD(device);

                    dispatcher.BeginInvoke(new Action(() =>
                        {
                            this.IsInitializing = false;
                            this.ResetScene(_sceneWidth, _sceneHeight);
                        }));

                })
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        this.LogError("error occurred while attaching render", t.Exception);
                    }
                });
        }


        public override void Dispose()
        {
            this.Detach();
            base.Dispose();
        }

        protected override void Detach()
        {
            //reset 
            Disposer.RemoveAndDispose(ref _depthStencil);
            Disposer.RemoveAndDispose(ref _quadRender);
            Disposer.RemoveAndDispose(ref _finalColorSurface);
            Disposer.RemoveAndDispose(ref _colorSurface);
            Disposer.RemoveAndDispose(ref _positionSurface);
            Disposer.RemoveAndDispose(ref _normalSurface);
            Disposer.RemoveAndDispose(ref _finalColorMap);
            Disposer.RemoveAndDispose(ref _colorMap);
            Disposer.RemoveAndDispose(ref _positionMap);
            Disposer.RemoveAndDispose(ref _normalMap);

            //model
            Disposer.RemoveAndDispose(ref _hullModelRenderer);
            Disposer.RemoveAndDispose(ref _chassisModelRenderer);
            Disposer.RemoveAndDispose(ref _turretModelRenderer);
            Disposer.RemoveAndDispose(ref _gunModelRenderer);


            //effect
            Disposer.RemoveAndDispose(ref _showGBufferEffect);
            Disposer.RemoveAndDispose(ref _clearGBufferEffect);
            Disposer.RemoveAndDispose(ref _renderGBufferEffect);
            Disposer.RemoveAndDispose(ref _lightingTankEffect);
            Disposer.RemoveAndDispose(ref _buildArmorColorEffect);
            //other
            Disposer.RemoveAndDispose(ref _armorColorMap);
            Disposer.RemoveAndDispose(ref _randomMap);
            Disposer.RemoveAndDispose(ref _smaa);

            this.LogInfo("hangar scene detached");
        }

        public override void RenderScene(DrawEventArgs args)
        {
            if (this.IsInitializing)
                return;

            UpdateTargetPositionAnimation(args.DeltaTime.TotalSeconds);

            TestUpdateModel(args.DeltaTime.TotalSeconds);


            var device = Renderer.Device;
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer | ClearFlags.Stencil, new Color(0xff, 0xff, 0xff, 0), 1.0f, 0);

            var dst = device.GetRenderTarget(0);

            device.BeginScene();

            if (ModelType == ModelType.Collision && _armorColorMapDirty)
            {
                UpdateArmorColorMap();
            }

            RenderGBuffer();

            if (GraphicsSettings.SMAAEnabled)
            {
                device.SetRenderTarget(0, _finalColorSurface);
            }
            else
            {
                device.SetRenderTarget(0, dst);
            }

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, new Color(0xff, 0xff, 0xff, 0), 1.0f, 0);

            device.SetTexture(0, _colorMap);
            device.SetTexture(1, _normalMap);
            device.SetTexture(2, _positionMap);
            device.SetTexture(3, _randomMap);

            if (ModelType == ModelType.Collision)
            {
                var parameters = this.CollisionModelRenderParameters;
                device.SetTexture(4, _armorColorMap);

                _lightingTankEffect.SetValue("tankThickestArmor", (float)parameters.TankThickestArmor + 0.125f);
                _lightingTankEffect.SetValue("tankThinnestArmor", (float)parameters.TankThinnestArmor - 0.125f);
                _lightingTankEffect.SetValue("tankThickestSpacingArmor", (float)parameters.TankThickestSpacingArmor + 0.125f);
                _lightingTankEffect.SetValue("tankThinnestSpacingArmor", (float)parameters.TankThinnestSpacingArmor - 0.125f);

                var regularArmorValueSelectionMax = (float)Math.Max(parameters.RegularArmorValueSelectionBegin, parameters.RegularArmorValueSelectionEnd);
                var regularArmorValueSelectionMin = (float)Math.Min(parameters.RegularArmorValueSelectionBegin, parameters.RegularArmorValueSelectionEnd);

                _lightingTankEffect.SetValue("regularArmorValueSelectionMax", regularArmorValueSelectionMax + 0.4f);
                _lightingTankEffect.SetValue("regularArmorValueSelectionMin", regularArmorValueSelectionMin - 0.4f);

                var spacingArmorValueSelectionMax = (float)Math.Max(parameters.SpacingArmorValueSelectionBegin, parameters.SpacingArmorValueSelectionEnd);
                var spacingArmorValueSelectionMin = (float)Math.Min(parameters.SpacingArmorValueSelectionBegin, parameters.SpacingArmorValueSelectionEnd);

                _lightingTankEffect.SetValue("spacingArmorValueSelectionMax", spacingArmorValueSelectionMax + 0.4f);
                _lightingTankEffect.SetValue("spacingArmorValueSelectionMin", spacingArmorValueSelectionMin - 0.4f);

                _lightingTankEffect.SetValue("hasRegularArmorHintValue", parameters.HasRegularArmorHintValue);
                _lightingTankEffect.SetValue("hasSpacingArmorHintValue", parameters.HasSpacingArmorHintValue);

                if (parameters.HasRegularArmorHintValue)
                {
                    _lightingTankEffect.SetValue("regularArmorHintValue", (float)parameters.RegularArmorHintValue);
                }

                if (parameters.HasSpacingArmorHintValue)
                {
                    _lightingTankEffect.SetValue("spacingArmorHintValue", (float)parameters.SpacingArmorHintValue);
                }

                _highLightValue = _maxHighlightDelta + _maxHighlightDelta * Math.Cos(args.TotalTime.TotalSeconds * 7.4);

                _lightingTankEffect.SetValue("highLightValue", (float)_highLightValue);

                _lightingTankEffect.SetValue("useBlackEdge", GraphicsSettings.CollisionModelStrokeEnabled);

                _lightingTankEffect.Technique = _lightingTankEffect.GetTechnique(1);
            }
            else
            {
                _lightingTankEffect.Technique = _lightingTankEffect.GetTechnique(0);
            }

            _lightingTankEffect.SetValue("useSSAO", GraphicsSettings.SSAOEnabled);

            _lightingTankEffect.Begin();
            _lightingTankEffect.BeginPass(0);

            _quadRender.Render(device);

            _lightingTankEffect.EndPass();
            _lightingTankEffect.End();

            RenderHUD(args);


            if (GraphicsSettings.SMAAEnabled)
            {
                device.SetRenderTarget(0, dst);
                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer | ClearFlags.Stencil, new Color(0xff, 0xff, 0xff, 0), 1.0f, 0);
                _smaa.Apply(_finalColorMap, _finalColorMap, dst);
            }

            if (_fps != null)
            {
                _fps.AddFrame(args.TotalTime);
                FPS = _fps.Value;
            }

            UpdateHitTest();

            Renderer.Device.EndScene();
        }

        private void UpdateHitTest()
        {
            if (IsHitTestEnabled && CameraMode == CameraMode.Sniper)
            {
                var ray = new Ray(_shotPosition, _targetPosition - _shotPosition);
                var invWorldTrans = Matrix.Invert(Transform);
                ray.Position = invWorldTrans.TransformCoord(ray.Position);
                ray.Direction = invWorldTrans.TransformNormal(ray.Direction);
                ray.Direction.Normalize();

                TryHitTestCollisionModel(ray);
            }
        }

        private void TestUpdateModel(double p)
        {
            //TurretRotationAngle = TurretRotationAngle + p ;
        }

        private bool TryPickTracer(Ray ray)
        {
            var result = new ProjectileTracerHitTestResult();
            foreach (var tracer in _projectileTracer)
            {
                tracer.HitTest(ray, ref result);
            }

            if (result.ProjectileTracerHits.Count == 0)
                return false;

            var closesetTracer = result.ClosesetHit.Value.Tracer;

            TryHitTestCollisionModel(closesetTracer.TracerRay);

            return true;
        }

        private void TryHitTestCollisionModel(Ray ray)
        {
            var result = new CollisionModelHitTestResult(ray);

            _hullModelRenderer.HitTest(ray, ref result);
            _turretModelRenderer.HitTest(ray, ref result);
            _gunModelRenderer.HitTest(ray, ref result);
            _chassisModelRenderer.HitTest(ray, ref result);

            _lastHitTestResult = result;

            var closesetArmorHit = result.ClosesetArmorHit;

            var parameters = this.CollisionModelRenderParameters;

            if (closesetArmorHit.HasValue)
            {
                if (closesetArmorHit.Value.Armor.IsSpacedArmor)
                {
                    var closesetSpacingArmorHit = closesetArmorHit.Value;

                    if (!parameters.HasSpacingArmorHintValue)
                        parameters.HasSpacingArmorHintValue = true;

                    if (parameters.SpacingArmorHintValue != closesetSpacingArmorHit.Armor.Thickness)
                        parameters.SpacingArmorHintValue = closesetSpacingArmorHit.Armor.Thickness;

                    var closesetRegularArmorHit = result.ClosesetRegularArmorHit;
                    if (closesetRegularArmorHit.HasValue)
                    {
                        if (!parameters.HasRegularArmorHintValue)
                            parameters.HasRegularArmorHintValue = true;

                        if (parameters.RegularArmorHintValue != closesetRegularArmorHit.Value.Armor.Thickness)
                            parameters.RegularArmorHintValue = closesetRegularArmorHit.Value.Armor.Thickness;
                    }
                    else
                    {
                        if (parameters.HasRegularArmorHintValue)
                            parameters.HasRegularArmorHintValue = false;
                    }
                }
                else
                {
                    if (parameters.HasSpacingArmorHintValue)
                        parameters.HasSpacingArmorHintValue = false;

                    var closesetRegularArmorHit = closesetArmorHit;
                    if (closesetRegularArmorHit.HasValue)
                    {
                        if (!parameters.HasRegularArmorHintValue)
                            parameters.HasRegularArmorHintValue = true;

                        if (parameters.RegularArmorHintValue != closesetRegularArmorHit.Value.Armor.Thickness)
                            parameters.RegularArmorHintValue = closesetRegularArmorHit.Value.Armor.Thickness;
                    }
                    else
                    {
                        if (parameters.HasRegularArmorHintValue)
                            parameters.HasRegularArmorHintValue = false;
                    }
                }

                if (!IsMouseOverModel)
                    IsMouseOverModel = true;
            }
            else
            {
                if (parameters.HasSpacingArmorHintValue)
                    parameters.HasSpacingArmorHintValue = false;
                if (parameters.HasRegularArmorHintValue)
                    parameters.HasRegularArmorHintValue = false;

                if (IsMouseOverModel)
                    IsMouseOverModel = false;
            }


            var shotTestResult = result.GetShootTestResult(TestShell);

            if (this.ShootTestResult != shotTestResult)
                ShootTestResult = shotTestResult;
        }

        private static Vector3 CalculateNormal(ref Vector3 p1, ref Vector3 p2, ref Vector3 p3)
        {
            float w0, w1, w2, v0, v1, v2, nx, ny, nz;
            w0 = p2.X - p1.X; w1 = p2.Y - p1.Y; w2 = p2.Z - p1.Z;
            v0 = p3.X - p1.X; v1 = p3.Y - p1.Y; v2 = p3.Z - p1.Z;
            nx = (w1 * v2 - w2 * v1);
            ny = (w2 * v0 - w0 * v2);
            nz = (w0 * v1 - w1 * v0);
            return new Vector3(nx, ny, nz);
        }


        public void AddProjectileTracer()
        {
            if (_lastHitTestResult == null || _lastHitTestResult.Hits.Count == 0)
                return;

            var tracer = new ProjectileTracer(Renderer.Device, _lastHitTestResult.HitRay, _lastHitTestResult.OrderedHits, TestShell);

            _projectileTracer.Add(tracer);
        }

        public void AddProjectileTracers()
        {
            if (_lastHitTestResult == null || _lastHitTestResult.Hits.Count == 0)
                return;

            var random = new Random();

            for (int i = 0; i != 20; ++i)
            {
                var randomDir = _lastHitTestResult.HitRay.Direction +
                    new Vector3(random.NextFloat(-0.05f, 0.05f), random.NextFloat(-0.05f, 0.05f), random.NextFloat(-0.05f, 0.05f));
                randomDir.Normalize();
                var ray = new Ray(_lastHitTestResult.HitRay.Position, randomDir);

                var result = new CollisionModelHitTestResult(ray);

                _hullModelRenderer.HitTest(ray, ref result);
                _turretModelRenderer.HitTest(ray, ref result);
                _gunModelRenderer.HitTest(ray, ref result);
                _chassisModelRenderer.HitTest(ray, ref result);

                if (result.Hits.Count != 0)
                {

                    var tracer = new ProjectileTracer(Renderer.Device, result.HitRay, result.OrderedHits, TestShell);

                    Monitor.Enter(_projectileTracer);
                    try
                    {
                        _projectileTracer.Add(tracer);
                    }
                    finally
                    {
                        Monitor.Exit(_projectileTracer);
                    }

                }
            }
        }

        private Ray GetPickRay(float x, float y)
        {
            var halfViewportWidth = (float)_width / 2;
            var halfViewportHeight = (float)_height / 2;
            var viewportCenterX = halfViewportWidth;
            var viewportCenterY = halfViewportHeight;

            var Xn = (x - viewportCenterX) / halfViewportWidth;
            var Yn = -(y - viewportCenterY) / halfViewportHeight;


            var px = Xn / _proj[0, 0];
            var py = Yn / _proj[1, 1];

            var ray = new Ray(Vector3.Zero, new Vector3(px, py, 1.0f));

            ray.Position = _invView.TransformCoord(ray.Position);
            ray.Direction = _invView.TransformNormal(ray.Direction);
            ray.Direction.Normalize();

            return ray;
        }

        void InvalidateArmorColorMap()
        {
            _armorColorMapDirty = true;
        }


        struct StopPointVertex
        {
            public float Offset;
            public ColorBGRA Color;
        }

        private void UpdateArmorColorMap()
        {
            this.LogInfo("update armor color map.");
            var device = Renderer.Device;

            Disposer.RemoveAndDispose(ref _armorColorMap);
            _armorColorMap = new Texture(device, 1024, 1, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);

            using (var _armorColorSurface = _armorColorMap.GetSurfaceLevel(0))
            {
                device.SetRenderTarget(0, _armorColorSurface);
                //

                using (var vertexDeclaration = new VertexDeclaration(device, new[]
                {
                    new VertexElement(0,0,DeclarationType.Float1,DeclarationMethod.Default,DeclarationUsage.Position,0),
                    new VertexElement(0,4,DeclarationType.Color,DeclarationMethod.Default,DeclarationUsage.Color,0),
                    VertexElement.VertexDeclarationEnd
                }))
                {
                    device.VertexDeclaration = vertexDeclaration;

                    var vertices = this.CollisionModelRenderParameters.RegularArmorSpectrumStops.Select(
                        (p) =>
                        {
                            return new StopPointVertex()
                            {
                                Offset = (float)p.Offset - 1.0f,
                                Color = new ColorBGRA(p.Color.R, p.Color.G, p.Color.B, p.Color.A)
                            };
                        }).ToArray();
                    var verticesSpacing = this.CollisionModelRenderParameters.SpacingArmorSpectrumStops.Select(
                    (p) =>
                    {
                        return new StopPointVertex()
                        {
                            Offset = (float)p.Offset,
                            Color = new ColorBGRA(p.Color.R, p.Color.G, p.Color.B, p.Color.A)
                        };
                    }).ToArray();

                    _buildArmorColorEffect.Technique = _buildArmorColorEffect.GetTechnique(0);
                    _buildArmorColorEffect.Begin();
                    _buildArmorColorEffect.BeginPass(0);
                    device.DrawUserPrimitives(PrimitiveType.LineStrip, 0, vertices.Length - 1, vertices);
                    device.DrawUserPrimitives(PrimitiveType.LineStrip, 0, verticesSpacing.Length - 1, verticesSpacing);
                    _buildArmorColorEffect.EndPass();
                    _buildArmorColorEffect.End();
                }
            }
            _armorColorMapDirty = false;
        }



        public void ClearAllProjectileTracer()
        {
            Monitor.Enter(_projectileTracer);
            try
            {
                _projectileTracer.Clear();
            }
            finally
            {
                Monitor.Exit(_projectileTracer);
            }
        }


    }
}

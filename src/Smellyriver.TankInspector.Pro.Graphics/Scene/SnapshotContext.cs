using System;
using System.Windows;
using SharpDX;
using SharpDX.Direct3D9;
using Smellyriver.TankInspector.Pro.Graphics.Frameworks;
using Smellyriver.TankInspector.Pro.Graphics.Smaa;

namespace Smellyriver.TankInspector.Pro.Graphics.Scene
{
    class SnapshotContext : IDisposable
    {
        Texture _normalMap;
        public Texture NormalMap { get { return _normalMap; } }

        Texture _positionMap;
        public Texture PositionMap { get { return _positionMap; } }

        Texture _colorMap;
        public Texture ColorMap { get { return _colorMap; } }

        Texture _finalColorMap;
        public Texture FinalColorMap { get { return _finalColorMap; } }

        Texture _dstMap;
        public Texture DstMap { get { return _dstMap; } }

        Surface _normalSurface;
        public Surface NormalSurface { get { return _normalSurface; } }

        Surface _positionSurface;
        public Surface PositionSurface { get { return _positionSurface; } }

        Surface _colorSurface;
        public Surface ColorSurface { get { return _colorSurface; } }

        Surface _finalColorSurface;
        public Surface FinalColorSurface { get { return _finalColorSurface; } }

        Surface _dstSurface;
        public Surface DstSurface { get { return _dstSurface; } }

        QuadRender _quadRender;
        public QuadRender QuadRender { get { return _quadRender; } }

        Texture _shootMap;
        public Texture ShootMap { get { return _shootMap; } }

        Surface _shootMapSurface;
        public Surface ShootMapSurface { get { return _shootMapSurface; } }

        Surface _depthStencil;
        public Surface DepthStencil { get { return _depthStencil; } }

        SMAA _smaa;
        public SMAA SMAA { get { return _smaa; } }

        public Matrix ViewProjection { get; private set; }

        public int Width { get; }
        public int Height { get; }

        public SnapshotContext(Device device, Matrix view, Rect rect, double sampleRatio)
        {
            var viewPort = device.Viewport;

            var halfWidth = viewPort.Width / 2;
            var halfHeight = viewPort.Height / 2;

            var centerX = (float)(2 * (rect.Left + rect.Width / 2)) / (float)viewPort.Width - 1.0f;
            var centerY = (float)(2 * (rect.Top + rect.Height / 2)) / (float)viewPort.Height - 1.0f;

            this.Width = (int)(rect.Width * sampleRatio);
            this.Height = (int)(rect.Height * sampleRatio);

            this.LogInfo("hangar scene shot to picture ({0},{1}) ", this.Width, this.Height);

            var scale = Math.Min((float)viewPort.Height / (float)rect.Height, (float)viewPort.Width / (float)rect.Width);


            var proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f,
                                               (float)this.Width / (float)this.Height, 0.1f, 707.0f)
                                                    * Matrix.Translation(-centerX, centerY, 0)
                                                    * Matrix.Scaling(scale, scale, 1f);
            this.ViewProjection = Matrix.Multiply(view, proj);

            _normalMap = new Texture(device, this.Width, this.Height, 1, Usage.RenderTarget, Format.A16B16G16R16F, Pool.Default);
            _positionMap = new Texture(device, this.Width, this.Height, 1, Usage.RenderTarget, Format.A16B16G16R16F, Pool.Default);
            _colorMap = new Texture(device, this.Width, this.Height, 1, Usage.RenderTarget, Format.A16B16G16R16F, Pool.Default);
            _finalColorMap = new Texture(device, this.Width, this.Height, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
            _dstMap = new Texture(device, this.Width, this.Height, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
            _normalSurface = _normalMap.GetSurfaceLevel(0);
            _positionSurface = _positionMap.GetSurfaceLevel(0);
            _colorSurface = _colorMap.GetSurfaceLevel(0);
            _finalColorSurface = _finalColorMap.GetSurfaceLevel(0);
            _dstSurface = _dstMap.GetSurfaceLevel(0);
            _quadRender = new QuadRender(device, this.Width, this.Height);
            _shootMap = new Texture(device, this.Width, this.Height, 1, Usage.None, Format.A8R8G8B8, Pool.SystemMemory);
            _shootMapSurface = _shootMap.GetSurfaceLevel(0);

            _depthStencil = Surface.CreateDepthStencil(device, this.Width, this.Height, Format.D24S8, MultisampleType.None, 0, true);
            device.DepthStencilSurface = _depthStencil;

            _smaa = new SMAA(device, 1, 1, SMAA.Preset.PRESET_ULTRA);

            _smaa.Reset(this.Width, this.Height, _quadRender);
        }

        public void Dispose()
        {
            Disposer.RemoveAndDispose(ref _shootMapSurface);
            Disposer.RemoveAndDispose(ref _shootMap);
            Disposer.RemoveAndDispose(ref _depthStencil);
            Disposer.RemoveAndDispose(ref _smaa);
            Disposer.RemoveAndDispose(ref _quadRender);
            Disposer.RemoveAndDispose(ref _dstSurface);
            Disposer.RemoveAndDispose(ref _finalColorSurface);
            Disposer.RemoveAndDispose(ref _colorSurface);
            Disposer.RemoveAndDispose(ref _positionSurface);
            Disposer.RemoveAndDispose(ref _normalSurface);
            Disposer.RemoveAndDispose(ref _dstMap);
            Disposer.RemoveAndDispose(ref _finalColorMap);
            Disposer.RemoveAndDispose(ref _colorMap);
            Disposer.RemoveAndDispose(ref _positionMap);
            Disposer.RemoveAndDispose(ref _normalMap);
        }
    }
}

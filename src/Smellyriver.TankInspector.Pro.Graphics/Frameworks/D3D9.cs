using System;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using SharpDX.Direct3D9;
using Smellyriver.TankInspector.Common;

namespace Smellyriver.TankInspector.Pro.Graphics.Frameworks
{
    public class D3D9 : D3D
    {
        /// <summary>
        /// 
        /// </summary>
        public D3D9()
        {
            PresentParameters presentparams = new PresentParameters();
            presentparams.Windowed = true;
            presentparams.SwapEffect = SwapEffect.Discard;
            presentparams.DeviceWindowHandle = D3D9.GetDesktopWindow();
            presentparams.EnableAutoDepthStencil = false;
            presentparams.AutoDepthStencilFormat = Format.D24S8;
            presentparams.PresentationInterval = PresentInterval.Default;

            try
            {
                using (Diagnostics.PotentialExceptionRegion)
                {
                    var context = new Direct3DEx();
                    _context = context;
                    this._device = new DeviceEx(context, 0, DeviceType.Hardware, IntPtr.Zero, CreateFlags.HardwareVertexProcessing | CreateFlags.Multithreaded | CreateFlags.FpuPreserve, presentparams);
                }
            }
            catch (Exception)
            {
                _context = new Direct3D();
                this._device = new Device(_context, 0, DeviceType.Hardware, IntPtr.Zero, CreateFlags.HardwareVertexProcessing | CreateFlags.Multithreaded | CreateFlags.FpuPreserve, presentparams);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disposer.RemoveAndDispose(ref _device);
                Disposer.RemoveAndDispose(ref _context);
                Disposer.RemoveAndDispose(ref _renderTarget);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDisposed { get { return _device == null; } }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = false)]
        static extern IntPtr GetDesktopWindow();

        /// <summary>
        /// 
        /// </summary>
        public Device Device { get { return _device.GetOrThrow(); } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public override void Reset(int w, int h)
        {
            _device.GetOrThrow();

            if (w < 1)
                throw new ArgumentOutOfRangeException(nameof(w));
            if (h < 1)
                throw new ArgumentOutOfRangeException(nameof(h));

            Disposer.RemoveAndDispose(ref _renderTarget);

            _renderTarget = new Texture(this._device, w, h, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);

            using (var surface = _renderTarget.GetSurfaceLevel(0))
                _device.SetRenderTarget(0, surface);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        protected T Prepared<T>(ref T property)
        {
            _device.GetOrThrow();
            if (property == null)
                Reset(1, 1);
            return property;
        }

        /// <summary>
        /// 
        /// </summary>
        public Texture RenderTarget { get { return Prepared(ref _renderTarget); } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dximage"></param>
        public override void SetBackBuffer(D3D9ImageSource dximage) 
        { 
            dximage.SetBackBuffer(RenderTarget); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override WriteableBitmap ToImage() { throw new NotImplementedException(); }

        protected Direct3D _context;
        protected Device _device;
        private   Texture _renderTarget;
    }
}

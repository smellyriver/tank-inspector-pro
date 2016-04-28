using SharpDX.Direct3D9;

namespace Smellyriver.TankInspector.Pro.Graphics.Scene
{

    class VertexState
    {
        public VertexBuffer VertexBuffer { get; set; }
        public VertexDeclaration VertexDeclaration { get; set; }
        public int Stride { get; set; }
        public int Count { get; set; }

        public void ApplyState(Device device)
        {
            device.SetStreamSource(0, VertexBuffer, 0, Stride);
            device.VertexDeclaration = VertexDeclaration;
        }
    };

}

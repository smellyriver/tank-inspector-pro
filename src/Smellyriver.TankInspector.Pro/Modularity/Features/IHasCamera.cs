using System;
using Smellyriver.TankInspector.Pro.Graphics;

namespace Smellyriver.TankInspector.Pro.Modularity.Features
{
    public interface IHasCamera : IFeature
    {
        Camera Camera { get; }
        event EventHandler CameraChanged;
    }
}

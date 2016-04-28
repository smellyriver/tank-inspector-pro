using System;
using Smellyriver.TankInspector.Pro.Data.Tank;

namespace Smellyriver.TankInspector.Pro.Modularity.Features
{
    public interface ITurretControllable : IFeature
    {
        TankInstance TankInstance { get; }

        event EventHandler TankInstanceChanged;
    }
}

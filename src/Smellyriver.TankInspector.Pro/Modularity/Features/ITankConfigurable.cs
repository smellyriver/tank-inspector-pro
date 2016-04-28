using System;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.Modularity.Features
{
    public interface ITankConfigurable : IFeature
    {
        IRepository Repository { get; }
        TankConfiguration TankConfiguration { get; }
        event EventHandler TankConfigurationChanged;
    }
}

using System;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.Modularity.Features
{
    public interface ICrewConfigurable : IFeature
    {
        IRepository Repository { get; }
        CrewConfiguration CrewConfiguration { get; }

        event EventHandler CrewConfigurationChanged;
    }
}

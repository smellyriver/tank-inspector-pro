using System;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.Modularity.Features
{
    public interface ICustomizationConfigurable : IFeature
    {
        IRepository Repository { get; }
        CustomizationConfiguration CustomizationConfiguration { get; }

        event EventHandler CustomizationConfigurationChanged;
    }
}

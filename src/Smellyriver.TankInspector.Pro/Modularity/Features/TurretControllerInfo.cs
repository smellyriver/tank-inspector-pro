using Smellyriver.TankInspector.Pro.Data.Entities;

namespace Smellyriver.TankInspector.Pro.Modularity.Features
{
    public class TurretControllerInfo
    {
        TurretYawLimits TurretYawLimits { get; set; }
        GunPitchLimitsComponent GunElevationLimits { get; set; }
        GunPitchLimitsComponent GunDepressionLimits { get; set; }
        
    }
}

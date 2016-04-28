using SharpDX;
using Smellyriver.TankInspector.Pro.Data.Entities;

namespace Smellyriver.TankInspector.Pro.Graphics
{
    static class ModuleExtensions
    {
        public static Vector3 GetHullPosition(this Chassis chassis)
        {
            return chassis.QueryVector3("hullPosition");
        }

        public static Vector3 GetTurretPosition(this Hull hull)
        {
            return hull.QueryVector3("turretPositions");
        }

        public static Vector3 GetGunPosition(this Turret turret)
        {
            return turret.QueryVector3("gunPosition");
        }
    }
}

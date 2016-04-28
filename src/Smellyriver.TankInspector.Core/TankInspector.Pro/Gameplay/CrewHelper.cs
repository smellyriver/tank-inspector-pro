using System;

namespace Smellyriver.TankInspector.Pro.Gameplay
{
    public static class CrewHelper
    {
        public static bool IsValidRole(string role)
        {
            return role == "commander"
                || role == "driver"
                || role == "gunner"
                || role == "loader"
                || role == "radioman";
        }

        public static void ThrowIfRoleInvalid(string role)
        {
            if (!CrewHelper.IsValidRole(role))
                throw new ArgumentException("role", "invalid role");
        }
    }
}

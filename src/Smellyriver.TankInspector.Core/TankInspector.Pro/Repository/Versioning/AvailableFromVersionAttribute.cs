using System;

namespace Smellyriver.TankInspector.Pro.Repository.Versioning
{
    public sealed class AvailableFromVersionAttribute : Attribute
    {
        public string Version { get;}

        public AvailableFromVersionAttribute(string version)
        {
            this.Version = version;
        }
    }
}

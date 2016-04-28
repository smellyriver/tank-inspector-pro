using System;

namespace Smellyriver.TankInspector.Pro.Repository.Versioning
{
    public sealed class ObsoleteOnVersionAttribute : Attribute
    {
        public string Version { get; }

        public ObsoleteOnVersionAttribute(string version)
        {
            this.Version = version;
        }
    }
}

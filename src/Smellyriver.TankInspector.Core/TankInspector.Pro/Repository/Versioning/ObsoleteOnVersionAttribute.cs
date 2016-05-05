using System;

namespace Smellyriver.TankInspector.Pro.Repository.Versioning
{
    public sealed class ObsoleteOnVersionAttribute : Attribute
    {
        private readonly string _version;

        public string Version
        {
            get { return _version; }
        }

        public ObsoleteOnVersionAttribute(string version)
        {
            this._version = version;
        }
    }
}

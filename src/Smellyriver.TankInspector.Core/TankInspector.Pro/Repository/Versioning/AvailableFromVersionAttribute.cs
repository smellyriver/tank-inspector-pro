using System;

namespace Smellyriver.TankInspector.Pro.Repository.Versioning
{
    public sealed class AvailableFromVersionAttribute : Attribute
    {
        private readonly string _version;

        public string Version
        {
            get { return _version; }
        }

        public AvailableFromVersionAttribute(string version)
        {
            this._version = version;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using log4net;
using Microsoft.Practices.Prism.Modularity;

namespace Smellyriver.TankInspector.Pro.Modularity
{
    public abstract class ModuleBase : IModule
    {
        private Dictionary<string, object> _metadatum;

        protected readonly ILog Log;
        protected bool IsDisposed { get; set; }

        public string Name
        {
            get { return this.L(this.GetMetadata("Name") as string) ?? this.L("modularity", "default_module_name"); }
        }

        public string Description
        {
            get { return this.L(this.GetMetadata("Description") as string) ?? this.L("modularity", "default_module_description"); }
        }

        public Version Version
        {
            get
            {
                var versionString = this.GetMetadata("Version") as string;
                Version version;
                if (Version.TryParse(versionString, out version))
                    return version;
                else
                    return this.GetType().Assembly.GetName().Version;
            }
        }

        public Guid Guid
        {
            get
            {
                var guidString = this.GetMetadata("Guid") as string;
                Guid guid;
                if (Guid.TryParse(guidString, out guid))
                    return guid;
                else
                    return Guid.Empty;
            }
        }

        public string Provider
        {
            get { return this.L(this.GetMetadata("Provider") as string) ?? this.L("modularity", "default_module_provider"); }
        }

        public ModuleBase()
        {
            this.ReadMetadatum();
            this.Log = SafeLog.GetLogger(this.Name);
        }

        private void ReadMetadatum()
        {
            _metadatum = this.GetType().GetCustomAttributes(typeof(ExportMetadataAttribute), false)
                                       .Cast<ExportMetadataAttribute>()
                                       .ToDictionary(a => a.Name, a => a.Value);
        }

        private object GetMetadata(string name)
        {
            object value;
            if (_metadatum.TryGetValue(name, out value))
                return value;

            return null;
        }

        public virtual void Initialize()
        {

        }

        ~ModuleBase()
        {
            this.InternalDispose(false);
        }

        public void Dispose()
        {
            this.InternalDispose(true);
            GC.SuppressFinalize(this);
        }

        private void InternalDispose(bool disposing)
        {
            if (this.IsDisposed)
                return;

            this.LogInfo("disposing {0} ({1}), version {2} ", this.Name, this.Guid, this.Version);
            this.Dispose(disposing);

            this.IsDisposed = true;
        }

        protected virtual void Dispose(bool disposing)
        {

        }


    }
}

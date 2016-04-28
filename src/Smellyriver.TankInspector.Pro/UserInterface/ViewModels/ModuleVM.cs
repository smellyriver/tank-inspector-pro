using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Practices.Prism.Modularity;

namespace Smellyriver.TankInspector.Pro.UserInterface.ViewModels
{
    class ModuleVM
    {
        public string Name { get; private set; }
        public string Version { get; private set; }
        public string Description { get; private set; }
        public string Provider { get; private set; }

        public IModule Model { get; private set; }

        private readonly Dictionary<string, object> _metadatum;

        public ModuleVM(IModule module)
        {
            this.Model = module;

            var moduleType = module.GetType();
            _metadatum = moduleType.GetCustomAttributes(typeof(ExportMetadataAttribute), false)
                                   .Cast<ExportMetadataAttribute>()
                                   .ToDictionary(a => a.Name, a => a.Value);

            var assembly = module.GetType().Assembly;

            this.Name = this.L(this.GetMetadata("Name") as string, assembly) ?? moduleType.Assembly.GetName().Name;
            this.Version = this.GetMetadata("Version") as string ?? moduleType.Assembly.GetName().Version.ToString();
            this.Description = this.L(this.GetMetadata("Description") as string, assembly);
            this.Provider = this.L(this.GetMetadata("Provider") as string, assembly) ?? this.L("modularity", "default_module_provider");
        }


        private object GetMetadata(string name)
        {
            object value;
            if (_metadatum.TryGetValue(name, out value))
                return value;

            return null;
        }

    }
}

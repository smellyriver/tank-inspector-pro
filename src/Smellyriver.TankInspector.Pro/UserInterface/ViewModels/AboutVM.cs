using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Prism.Modularity;

namespace Smellyriver.TankInspector.Pro.UserInterface.ViewModels
{
    class AboutVM
    {
        public string ProductVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        public IEnumerable<ModuleVM> Modules { get; private set; }

        public AboutVM()
        {
            var modules = App.CompositionContainer.GetExportedValues<IModule>();

            this.Modules = modules.Select(m => new ModuleVM(m)).ToArray();
        }
    }
}

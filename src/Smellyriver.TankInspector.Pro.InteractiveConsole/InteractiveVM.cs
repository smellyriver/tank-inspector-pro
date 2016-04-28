using System.IO;
using System.Reflection;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Pro.InteractiveConsole.Automation;
using Smellyriver.TankInspector.Pro.InteractiveConsole.Scripting;
using Smellyriver.TankInspector.Pro.InteractiveConsole.Scripting.IronPython;

namespace Smellyriver.TankInspector.Pro.InteractiveConsole
{
    class InteractiveVM : NotificationObject
    {

        public IScriptingContext Context { get; private set; }


        public InteractiveVM()
        {
            var scriptingContext = new IronPythonScriptingContext();
            var automationContext = new AutomationContext (scriptingContext);
            AutomationContext.Current = automationContext;

            scriptingContext.InjectGlobalVariable("automation_context", automationContext);
            scriptingContext.LoadEmbeddedScript(Assembly.GetExecutingAssembly(), "Automation/Scripts/(Initialize).py");
            scriptingContext.LoadEmbeddedScript(Assembly.GetExecutingAssembly(), "Automation/Scripts/Globals.py");
            scriptingContext.ExternalScriptPaths.Add(Path.GetDirectoryName("Scripts"));
            this.Context = scriptingContext;
        }

    }


}

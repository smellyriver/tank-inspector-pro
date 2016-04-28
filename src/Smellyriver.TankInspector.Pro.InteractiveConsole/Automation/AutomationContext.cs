using System;
using System.Reflection;
using Smellyriver.TankInspector.Pro.InteractiveConsole.Scripting;

namespace Smellyriver.TankInspector.Pro.InteractiveConsole.Automation
{
    public class AutomationContext
    {
        internal static AutomationContext Current { get; set; }

        /// <summary>
        /// Gets or sets the current scripting context.
        /// </summary>
        /// <value>The scripting context.</value>
        public IScriptingContext ScriptingContext { get; set; }


        public AutomationContext(IScriptingContext scriptingContext)
        {
            this.ScriptingContext = scriptingContext;
        }

        /// <summary>
        /// Exits the application.
        /// </summary>
        public void Exit()
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Show application version
        /// </summary>
        public Version Version()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
}

using System.Reflection;

namespace Smellyriver.TankInspector.Pro.InteractiveConsole.Scripting.IronPython
{
    /// <summary>
    /// Extends the IScriptingContext with support for interoperability specific to IronPython.
    /// </summary>
    public interface IIronPythonScriptingContext : IScriptingContext
    {
        /// <summary>
        /// Pushes a variable into the current scripting context as a global variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="variable">The variable.</param>
        void InjectGlobalVariable(string name, object variable);

        /// <summary>
        /// Extracts a script file from an embedded resource in an assembly and loads it into the current scripting context.
        /// </summary>
        /// <param name="containingAssembly">The assembly containing the script file.</param>
        /// <param name="scriptFile">The script file, specified as a file name, for example: <c>Scripts/Foo.py</c>.</param>
        void LoadEmbeddedScript(Assembly containingAssembly, string scriptFile);
    }
}
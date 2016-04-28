
using System;

namespace Smellyriver.TankInspector.Pro.InteractiveConsole.Scripting
{
    /// <summary>
    /// Represents the output stream writer used by the console.
    /// </summary>
    /// <remarks>
    /// Should this just inherit TextWriter?
    /// </remarks>
    public interface IScriptOutputWriter
    {
        ConsoleColor? ForegroundColor { get; set; }
        void Clear();
        void EnsureNewLine();
        void Write(string text);
        void WriteLine(string text);
    }
}

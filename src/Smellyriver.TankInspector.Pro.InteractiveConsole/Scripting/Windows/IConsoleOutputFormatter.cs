using System;
using System.Collections.Generic;

namespace Smellyriver.TankInspector.Pro.InteractiveConsole.Scripting.Windows
{
    /// <summary>
    /// Implemented by classes which provide custom output formatting behavior.
    /// </summary>
    public interface IConsoleOutputFormatter
    {
        /// <summary>
        /// Formats the specified raw output from the scripting context.
        /// </summary>
        /// <param name="text">The raw text.</param>
        /// <param name="currentConsoleColor">The current scripting context foreground color.</param>
        /// <param name="mappings">Shell-specific console color/brush mapping table.</param>
        /// <returns>A formatted text structure providing information about the text to display.</returns>
        FormattedOutput Format(string text, ConsoleColor? currentConsoleColor, IEnumerable<ConsoleColorMapping> mappings);

        /// <summary>
        /// Clears any knowledge of previous formatting.
        /// </summary>
        void Clear();
    }
}
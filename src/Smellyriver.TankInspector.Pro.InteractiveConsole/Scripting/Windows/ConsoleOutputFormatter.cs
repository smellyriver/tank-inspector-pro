using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Media;

namespace Smellyriver.TankInspector.Pro.InteractiveConsole.Scripting.Windows
{
    /// <summary>
    /// A default shell output formatter with the ability to delay newlines.
    /// </summary>
    public class ConsoleOutputFormatter : IConsoleOutputFormatter
    {
        private bool _lastNewlineWasDelayed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleOutputFormatter"/> class.
        /// </summary>
        public ConsoleOutputFormatter()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether to delay newlines.
        /// </summary>
        /// <value><c>true</c> if newlines should be delayed; otherwise, <c>false</c>.</value>
        public bool DelayNewlines { get; set; }

        /// <summary>
        /// Formats the specified raw output from the scripting context.
        /// </summary>
        /// <param name="text">The raw text.</param>
        /// <param name="currentConsoleColor">The current scripting context foreground color.</param>
        /// <param name="mappings">Shell-specific console color/brush mapping table.</param>
        /// <returns>
        /// A formatted text structure providing information about the text to display.
        /// </returns>
        public FormattedOutput Format(string text, ConsoleColor? currentConsoleColor, IEnumerable<ConsoleColorMapping> mappings)
        {
            // Decide whether to delay newlines
            if (_lastNewlineWasDelayed)
            {
                text = Environment.NewLine + text;
            }
            _lastNewlineWasDelayed = false;
            if (DelayNewlines)
            {
                if (text.EndsWith(Environment.NewLine))
                {
                    _lastNewlineWasDelayed = true;
                    text = text.Substring(0, text.LastIndexOf(Environment.NewLine));
                }
            }

            // Choose the appropriate color
            var brush = null as Brush;
            if (mappings != null && currentConsoleColor != null)
            {
                foreach (var mapping in mappings)
                {
                    if (mapping == null) continue;

                    if (mapping.ConsoleColor == currentConsoleColor)
                    {
                        brush = mapping.Brush;
                    }
                }
            }

            // Create the run
            var run = new Run(text);
            if (brush != null) run.Foreground = brush;
            return new FormattedOutput(text, run);
        }

        /// <summary>
        /// Clears any knowledge of previous formatting.
        /// </summary>
        public void Clear()
        {
            _lastNewlineWasDelayed = false;
        }
    }
}
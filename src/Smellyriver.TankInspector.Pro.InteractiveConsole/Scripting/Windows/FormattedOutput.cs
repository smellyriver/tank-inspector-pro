using System.Collections.Generic;
using System.Windows.Documents;

namespace Smellyriver.TankInspector.Pro.InteractiveConsole.Scripting.Windows
{
    /// <summary>
    /// Reprents raw text that has been formatted.
    /// </summary>
    public class FormattedOutput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedOutput"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="inlines">The inlines.</param>
        public FormattedOutput(string text, params Inline[] inlines)
        {
            Text = text;
            Inlines = inlines;
        }

        /// <summary>
        /// Gets or sets the text version of the output to display.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; private set; }

        /// <summary>
        /// Gets or sets the rich text inlines to display.
        /// </summary>
        /// <value>The inlines.</value>
        public IEnumerable<Inline> Inlines { get; private set; }
    }
}

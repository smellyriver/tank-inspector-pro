using System.ComponentModel;

namespace Smellyriver.TankInspector.Pro.InteractiveConsole.Scripting.Windows
{
    /// <summary>
    /// Captures and stores a history of commands executed against a shell.
    /// </summary>
    public class CommandHistory : BindingList<string>
    {
        private int _next;

        /// <summary>
        /// Gets or sets the pointer to the next command history item.
        /// </summary>
        /// <value>The next.</value>
        protected int Next
        {
            get { return _next; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                if (value > Count)
                {
                    value = Count;
                }
                _next = value;
            }
        }

        private string GetNext()
        {
            if (Next >= 0 && Next < Count)
            {
                return this[Next];
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the command executed before the current command.
        /// </summary>
        /// <returns></returns>
        public string Up()
        {
            Next--;
            return GetNext();
        }

        /// <summary>
        /// Gets the command executed after the current command, or an empty string if we are at the bottom of the list.
        /// </summary>
        /// <returns></returns>
        public string Down()
        {
            Next++;
            return GetNext();
        }

        /// <summary>
        /// Raises the <see cref="E:System.ComponentModel.BindingList`1.ListChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.ListChangedEventArgs"/> that contains the event data.</param>
        protected override void OnListChanged(ListChangedEventArgs e)
        {
            base.OnListChanged(e);
            Next = Count;
        }
    }
}
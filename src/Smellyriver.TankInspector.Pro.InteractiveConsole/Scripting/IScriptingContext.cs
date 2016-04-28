using System.ComponentModel;

namespace Smellyriver.TankInspector.Pro.InteractiveConsole.Scripting
{
    /// <summary>
    /// Implemented by classes which provide access to 
    /// </summary>
    public interface IScriptingContext : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the current prompt string.
        /// </summary>
        /// <value>The prompt.</value>
        string Prompt { get; }
        
        /// <summary>
        /// Verifies the command. This allows the scripting context to parse a command and reject it instead of executing it.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>True if the command should be executed, otherwise false.</returns>
        bool VerifyCommand(string command);

        /// <summary>
        /// Executes the command line. 
        /// </summary>
        /// <param name="command">The command.</param>
        void ExecuteCommand(string command);

        /// <summary>
        /// Adds an output writer which will receive output from the scripting context.
        /// </summary>
        /// <param name="writer">The writer.</param>
        void AddOutputWriter(IScriptOutputWriter writer);

        /// <summary>
        /// Removes the output writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        void RemoveOutputWriter(IScriptOutputWriter writer);

        /// <summary>
        /// Print objects to output
        /// </summary>
        void Print(params object[] objects);
    }
}

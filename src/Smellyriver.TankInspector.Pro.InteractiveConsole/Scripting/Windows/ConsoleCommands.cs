using System.Windows.Input;

namespace Smellyriver.TankInspector.Pro.InteractiveConsole.Scripting.Windows
{
    public static class ConsoleCommands
    {
        public static RoutedUICommand Clear = new RoutedUICommand("Clear", "Clear", typeof(ConsoleCommands));
        public static RoutedUICommand Execute = new RoutedUICommand("Execute", "Execute", typeof(ConsoleCommands));
        public static RoutedUICommand CommandHistoryUp = new RoutedUICommand("CommandHistoryDown", "History Down", typeof(ConsoleCommands));
        public static RoutedUICommand CommandHistoryDown = new RoutedUICommand("CommandHistoryDown", "History Down", typeof(ConsoleCommands));
    }
}

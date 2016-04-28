using System.Windows.Input;

namespace Smellyriver.TankInspector.Pro.Input
{
    public static class ApplicationCommands
    {
        public static readonly RoutedUICommand ManageClients = new RoutedUICommand();
        public static readonly RoutedUICommand ShowAboutDialog = new RoutedUICommand();
        public static readonly RoutedUICommand Exit = new RoutedUICommand();
    }
}

using System.Windows.Input;
using Smellyriver.TankInspector.Pro.Globalization;

namespace Smellyriver.TankInspector.Pro.InternalModules
{
    static class InternalModuleCommands
    {
        public static readonly RoutedUICommand ManageGameClients 
            = new RoutedUICommand(Localization.Instance.L("game_client_manager", "manage_game_clients_menu_item"), 
                                  "manageGameClients", 
                                  typeof(InternalModuleCommands));
    }
}

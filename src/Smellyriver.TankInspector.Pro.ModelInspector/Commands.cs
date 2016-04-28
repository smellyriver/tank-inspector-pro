using System.Windows.Input;

namespace Smellyriver.TankInspector.Pro.ModelInspector
{
    public static class Commands
    {
        public static readonly RoutedUICommand ExportTankMesh = new RoutedUICommand();
        public static readonly RoutedUICommand ExportUVMap = new RoutedUICommand();
        public static readonly RoutedUICommand ExportTextures = new RoutedUICommand();
        public static readonly RoutedUICommand SwitchToUndamagedModel = new RoutedUICommand();
        public static readonly RoutedUICommand SwitchToDestroyedModel = new RoutedUICommand();
        public static readonly RoutedUICommand SwitchToExplodedModel = new RoutedUICommand();
        public static readonly RoutedUICommand SwitchToNormalTextureMode = new RoutedUICommand();
        public static readonly RoutedUICommand SwitchToGridTextureMode = new RoutedUICommand();
        public static readonly RoutedUICommand SwitchToOfficialTextureSource = new RoutedUICommand();
        public static readonly RoutedUICommand SwitchToModTextureSource = new RoutedUICommand();
        public static readonly RoutedUICommand ToggleCamouflage = new RoutedUICommand();
    }
}

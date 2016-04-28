using System.Windows.Input;

namespace Smellyriver.TankInspector.Pro.ModelShared
{
    public static class Commands
    {
        public const int WireframeGroupPriority = 0;
        public const int ShowModulesGroupPriority = 100;
        public const int ShowFPSGroupPriority = 200;

        public static readonly RoutedUICommand Snapshot = new RoutedUICommand();
        public static readonly RoutedUICommand SwitchToSolidMode = new RoutedUICommand();
        public static readonly RoutedUICommand SwitchToWireframeMode = new RoutedUICommand();
        public static readonly RoutedUICommand ToggleShowChassis = new RoutedUICommand();
        public static readonly RoutedUICommand ToggleShowHull = new RoutedUICommand();
        public static readonly RoutedUICommand ToggleShowTurret = new RoutedUICommand();
        public static readonly RoutedUICommand ToggleShowGun = new RoutedUICommand();
        public static readonly RoutedUICommand ToggleFPSDisplay = new RoutedUICommand();
        public static readonly RoutedUICommand ToggleTriangleCountDisplay = new RoutedUICommand();
    }
}

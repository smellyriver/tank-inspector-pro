using System.Windows.Input;

namespace Smellyriver.TankInspector.Pro.Input
{
    public static class ModelCommands
    {
        public static readonly RoutedUICommand ExportTankMesh = new RoutedUICommand();
        public static readonly RoutedUICommand ExportUVMap = new RoutedUICommand();
        public static readonly RoutedUICommand ExportTextures = new RoutedUICommand();
        public static readonly RoutedUICommand Snapshot = new RoutedUICommand();
        public static readonly RoutedUICommand ToggleUndamagedModel = new RoutedUICommand();
        public static readonly RoutedUICommand ToggleCollisionModel = new RoutedUICommand();
        public static readonly RoutedUICommand ToggleDestroyedModel = new RoutedUICommand();
        public static readonly RoutedUICommand ToggleHDModel = new RoutedUICommand();
        public static readonly RoutedUICommand ToggleSolidMode = new RoutedUICommand();
        public static readonly RoutedUICommand ToggleWireframeMode = new RoutedUICommand();
        public static readonly RoutedUICommand ToggleShowChassis = new RoutedUICommand();
        public static readonly RoutedUICommand ToggleShowHull = new RoutedUICommand();
        public static readonly RoutedUICommand ToggleShowTurret = new RoutedUICommand();
        public static readonly RoutedUICommand ToggleShowGun = new RoutedUICommand();
        public static readonly RoutedUICommand ToggleNormalTextureMode = new RoutedUICommand();
        public static readonly RoutedUICommand ToggleGridTextureMode = new RoutedUICommand();
        public static readonly RoutedUICommand ToggleOfficialTextureSource = new RoutedUICommand();
        public static readonly RoutedUICommand ToggleModTextureSource = new RoutedUICommand();
        public static readonly RoutedUICommand ToggleCamouflage = new RoutedUICommand();
        public static readonly RoutedUICommand ToggleFPSDisplay = new RoutedUICommand();
        public static readonly RoutedUICommand ToggleTriangleCountDisplay = new RoutedUICommand();
    }
}


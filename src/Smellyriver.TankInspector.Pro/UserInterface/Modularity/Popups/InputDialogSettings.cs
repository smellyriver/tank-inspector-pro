using MahApps.Metro.Controls.Dialogs;

namespace Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups
{
    public class InputDialogSettings : DialogSettings
    {
        public string DefaultText { get; set; }

        internal override MetroDialogSettings ToMetroDialogSettings()
        {
            var result = base.ToMetroDialogSettings();
            result.DefaultText = this.DefaultText;
            result.ColorScheme = MetroDialogColorScheme.Accented;
            return result;
        }
    }
}

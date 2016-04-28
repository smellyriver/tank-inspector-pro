using MahApps.Metro.Controls.Dialogs;

namespace Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups
{
    public class DialogSettings
    {
        public string AffirmativeButtonText { get; set; }
        public string NegativeButtonText { get; set; }
        public string FirstAuxiliaryButtonText { get; set; }
        public string SecondAuxiliaryButtonText { get; set; }

        public bool UseAnimationOnShow { get; set; }
        public bool UseAnimationOnHide { get; set; }

        public DialogSettings()
        {
            this.AffirmativeButtonText = this.L("common", "ok");
            this.NegativeButtonText = this.L("common", "cancel");
            this.UseAnimationOnShow = true;
            this.UseAnimationOnHide = true;
        }

        internal virtual MetroDialogSettings ToMetroDialogSettings()
        {
            return new MetroDialogSettings()
            {
                AffirmativeButtonText = this.AffirmativeButtonText,
                NegativeButtonText = this.NegativeButtonText,
                FirstAuxiliaryButtonText = this.FirstAuxiliaryButtonText,
                SecondAuxiliaryButtonText = this.SecondAuxiliaryButtonText,
                AnimateShow = this.UseAnimationOnShow,
                AnimateHide = this.UseAnimationOnHide,
                ColorScheme = MetroDialogColorScheme.Theme
            };
        }
    }
}

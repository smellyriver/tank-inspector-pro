using System.Text;
using System.Windows.Media;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Globalization;
using Smellyriver.TankInspector.Pro.TankModuleShared;

namespace Smellyriver.TankInspector.Pro.TankConfigurator
{
    class ComponentVM : NotificationObject
    {

        public static string GetGunDescription(Gun gun)
        {
            var penetrationBuilder = new StringBuilder();
            var damageBuilder = new StringBuilder();
            foreach (var shell in gun.Ammunition)
            {
                penetrationBuilder.Append(shell.Penetration100)
                                  .Append("/");
                damageBuilder.Append(shell.Damage)
                             .Append("/");
            }

            penetrationBuilder.Remove(penetrationBuilder.Length - 1, 1);
            damageBuilder.Remove(damageBuilder.Length - 1, 1);

            return Localization.Instance.L("tank_configurator",
                                           "gun_description",
                                           penetrationBuilder.ToString(),
                                           damageBuilder.ToString());
        }

        public static string BuildArmorString(Turret turret)
        {
            return string.Format("{0}/{1}/{2}",
                                 turret.FrontArmor,
                                 turret.SideArmor,
                                 turret.RearArmor);
        }

        public static string GetTurretDescription(Turret turret)
        {
            if (!turret.IsPrimaryArmorDefined)
                return null;
            return Localization.Instance.L("tank_configurator",
                                           "turret_description",
                                           ComponentVM.BuildArmorString(turret));
        }

        public static string GetEngineDescription(Engine engine)
        {
            return Localization.Instance.L("tank_configurator",
                                           "engine_description",
                                           engine.Power,
                                           engine.ChanceOnFire.ToString("#%"));
        }

        public static string GetRadioDescription(Radio radio)
        {
            return Localization.Instance.L("tank_configurator",
                                           "radio_description",
                                           radio.SignalDistance);
        }

        public static string GetChassisDescription(Chassis chassis)
        {
            return Localization.Instance.L("tank_configurator",
                                           "chassis_description",
                                           chassis.TraverseSpeed,
                                           (chassis.MaximumLoad / 1000.0).ToString("#,0.###"));
        }

        public static string GetShellDescription(Shell shell)
        {
            return Localization.Instance.L("tank_configurator",
                                           "shell_description",
                                           shell.Penetration100,
                                           shell.Damage);
        }

        public static string GetAccessoryDescription(Accessory accessory)
        {
            return accessory.Description;
        }

        public static string GetDescription(Component component)
        {
            switch (component.ElementName)
            {
                case "gun":
                    return ComponentVM.GetGunDescription((Gun)component);
                case "turret":
                    return ComponentVM.GetTurretDescription((Turret)component);
                case "engine":
                    return ComponentVM.GetEngineDescription((Engine)component);
                case "radio":
                    return ComponentVM.GetRadioDescription((Radio)component);
                case "chassis":
                    return ComponentVM.GetChassisDescription((Chassis)component);
                case "shell":
                    return ComponentVM.GetShellDescription((Shell)component);
                case "consumable":
                case "equipment":
                    return ComponentVM.GetAccessoryDescription((Accessory)component);

            }

            return null;
        }


        public ImageSource Icon { get; set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        private Component _model;
        public Component Model
        {
            get { return _model; }
            set
            {
                _model = value;
                this.Name = _model.Name;
                this.Icon = ModuleIcon.GetModuleIcon(_model.ElementName);
                this.Description = ComponentVM.GetDescription(_model);
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                this.RaisePropertyChanged(() => this.IsEnabled);
            }
        }


        public bool IsElite { get; private set; }

        public object ToolTip { get; private set; }

        public ComponentVM(Component component, bool isElite)
        {
            this.Model = component;
            this.IsEnabled = true;
            this.IsElite = isElite;
        }

        public void NotifyToolTipChanged()
        {
            this.RaisePropertyChanged(() => this.ToolTip);
        }

    }

}

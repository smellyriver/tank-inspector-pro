using Smellyriver.TankInspector.Pro.Globalization;
using Smellyriver.TankInspector.Pro.Graphics.Scene;

namespace Smellyriver.TankInspector.Pro.ModelShared
{
    public class RotationCenterVM
    {
        public static RotationCenterVM[] RotationCenters
        {
            get
            {
                return new[]
                {
                    new RotationCenterVM(RotationCenter.Hull, 
                                         Localization.Instance.L("model_shared", "rotation_center_hull"), 
                                         Localization.Instance.L("model_shared", "rotation_center_hull_description")),
                    new RotationCenterVM(RotationCenter.Turret, 
                                         Localization.Instance.L("model_shared", "rotation_center_turret"), 
                                         Localization.Instance.L("model_shared", "rotation_center_turret_description")),
                };
            }
        }

        public RotationCenter Model { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        public RotationCenterVM(RotationCenter mode, string name, string description)
        {
            this.Model = mode;
            this.Name = name;
            this.Description = description;
        }
    }
}

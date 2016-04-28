using Smellyriver.TankInspector.Pro.Data.Entities;

namespace Smellyriver.TankInspector.Pro.Graphics.Scene
{
    struct CollisionModelHit
    {
        public float Distance;
        public ArmorGroup Armor;
        public float InjectionCosine { get; set; }
        public ModuleMesh Mesh { get; set; }
    }

}

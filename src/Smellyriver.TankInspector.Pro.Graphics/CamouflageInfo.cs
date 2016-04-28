using SharpDX;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Data.Tank;

namespace Smellyriver.TankInspector.Pro.Graphics
{
    public sealed class CamouflageInfo
    {

        private readonly TankInstance _tankInstance;


        public string ExclusionMask { get; private set; }

        public Vector4 Tiling
        {
            get { return _tankInstance.QueryVector4("camouflage/tiling"); }
        }

        public string Texture
        {
            get { return _tankInstance["camouflage/texture"]; }
        }

        public Color[] Colors
        {
            get
            {
                return new[]
                {
                    _tankInstance.QueryColor("camouflage/colors/c0"),
                    _tankInstance.QueryColor("camouflage/colors/c1"),
                    _tankInstance.QueryColor("camouflage/colors/c2"),
                    _tankInstance.QueryColor("camouflage/colors/c3"),
                };
            }
        }

        public Color MetallicColor
        {
            get { return _tankInstance.QueryColor("camouflage/metallic"); }
        }

        public Color GlossColor
        {
            get { return _tankInstance.QueryColor("camouflage/gloss"); }
        }


        private CamouflageInfo(TankInstance tankInstance, string exclusionMask)
        {
            _tankInstance = tankInstance;
            this.ExclusionMask = exclusionMask;
        }

        public CamouflageInfo(TankInstance tankInstance)
            : this(tankInstance, tankInstance["camouflage/exclusionMask"])
        {
            
        }

        public CamouflageInfo(TankInstance tankInstance, Module module)
            : this(tankInstance, module["camouflage/exclusionMask"])
        {

        }
    }
}

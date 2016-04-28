using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D9;

namespace Smellyriver.TankInspector.Pro.Graphics.Scene
{

    class RenderGroup : MeshGroup
    {
        public VertexState VertexState { get; set; }
        public IndexBuffer Indices { get; set; }
        public int VerticesCount { get; set; }
        public int MinVertexIndex { get; set; }
        public IDictionary<string, Texture> Textures { get; set; }
        public bool AlphaTestEnable { get; set; }
        public bool UseNormalPackDXT1 { get; set; }
        public float DetailPower { get; set; }
        public Vector4 DetailUVTiling { get; set; }
        public int AlphaReference { get; set; }
        public bool RenderArmor { get; set; }
        public bool UseNormalBC1 { get; set; }


        private readonly GraphicsSettings _graphicsSettings;

        public RenderGroup(GraphicsSettings settings)
        {
            _graphicsSettings = settings;
            UseNormalPackDXT1 = true;
        }

        public void ApplyState(Device device, Effect effect)
        {
            VertexState.ApplyState(device);
            device.Indices = Indices;

            if (RenderArmor)
            {
                effect.SetValue("useDiffuse", false);
                effect.SetValue("useNormal", false);
                effect.SetValue("useSpecular", false);
                effect.SetValue("useArmor", true);
                effect.SetValue("spacingArmor", this.ArmorGroup == null ? false : this.ArmorGroup.IsSpacedArmor);
                effect.SetValue("armorValue", this.ArmorGroup == null ? 0f : (float)this.ArmorGroup.Thickness);
                effect.SetValue("armorChanceToHitByProjectile", this.ArmorGroup == null ? 0f : (float)ArmorGroup.ChanceToHitByProjectile);

            }
            else
            {
                BindTexture(device, effect);
                effect.SetValue("useArmor", false);
            }
        }

        private void BindTexture(Device device, Effect effect)
        {
            if (Textures.ContainsKey("diffuseMap"))
            {
                device.SetTexture(0, Textures["diffuseMap"]);
                effect.SetValue("useDiffuse", true);
            }
            else
            {
                effect.SetValue("useDiffuse", false);
            }

            if (Textures.ContainsKey("normalMap") && _graphicsSettings.NormalTextureEnabled)
            {
                device.SetTexture(1, Textures["normalMap"]);
                effect.SetValue("useNormal", true);
            }
            else
            {
                effect.SetValue("useNormal", false);
            }

            if (_graphicsSettings.SpecularTextureEnabled)
            {
                if (Textures.ContainsKey("specularMap"))
                {
                    device.SetTexture(2, Textures["specularMap"]);
                    effect.SetValue("useSpecular", true);
                    effect.SetValue("useMetallicDetail", false);

                }
                else if (Textures.ContainsKey("metallicGlossMap"))
                {
                    device.SetTexture(2, Textures["metallicGlossMap"]);
                    effect.SetValue("useSpecular", true);
                    if (Textures.ContainsKey("metallicDetailMap") && DetailPower > 0.0f)
                    {
                        device.SetTexture(3, Textures["metallicDetailMap"]);
                        effect.SetValue("useMetallicDetail", true);
                        effect.SetValue("detailUVTiling", DetailUVTiling);
                        effect.SetValue("detailPower", DetailPower);
                    }
                }

            }
            else
            {
                effect.SetValue("useSpecular", false);
            }


            effect.SetValue("alphaTestEnable", AlphaTestEnable);

            if (AlphaTestEnable)
                effect.SetValue("alphaReference", (float)AlphaReference / 255.0f);

            effect.SetValue("useNormalPackDXT1", UseNormalPackDXT1);

            effect.SetValue("useNormalBC1", UseNormalBC1);
        }




    };

}

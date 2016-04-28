using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using SharpDX.Direct3D9;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Graphics.Frameworks;
using Smellyriver.TankInspector.Pro.IO;

namespace Smellyriver.TankInspector.Pro.Graphics.Scene
{
    partial class ModuleMesh : SceneMeshBase
    {

        private readonly Device _device;

        public Tank Tank { get; private set; }

        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsSettings _graphicsSettings;

        public IEnumerable<MeshGroup> Groups { get { return _renderGroups; } }

        private readonly List<RenderGroup> _renderGroups = new List<RenderGroup>();

        public ModuleMesh(ModuleModel model, IFileLocator fileLocator, Device device, GraphicsSettings graphicsSettings)
        {
            _graphicsSettings = graphicsSettings;

            this.LogInfo("load tank mesh {0} from {1}", model.ModelName, model.TankInstance.Tank.Name);

            this.Tank = model.TankInstance.Tank;
            _device = device;

            if (model.Visual == null)
                throw new ArgumentException("model.Visual is null", nameof(model));

            if (model.Primitives == null)
                throw new ArgumentException("model.Primitives is null", nameof(model));

            var verticesMap = model.Primitives.Vertices.ToDictionary(k => k.Key, k => ModuleMesh.ConvertToVertexBuffer(k.Value, device));
            var indicesMap = model.Primitives.Indices.ToDictionary(k => k.Key, k => ModuleMesh.ConvertToIndexBuffer(k.Value, device));

            foreach (var renderSet in model.Visual.RenderSets)
            {
                //renderSet.Geometry.PrimitiveName
                var vState = verticesMap[renderSet.Geometry.VerticesName];
                var indices = indicesMap[renderSet.Geometry.IndicesName];
                var rawVertices = model.Primitives.Vertices[renderSet.Geometry.VerticesName].Vertices;
                var rawIndices = model.Primitives.Indices[renderSet.Geometry.IndicesName];

                foreach (var group in renderSet.Geometry.ModelPrimitiveGroups.Values)
                {
                    RenderGroup renderGroup;

                    if (group.Sectioned)
                    {
                        renderGroup = new RenderGroup(graphicsSettings)
                        {
                            MinVertexIndex = (int)group.StartVertex,
                            VerticesCount = (int)group.VerticesCount,
                            StartIndex = (int)group.StartIndex,
                            PrimitiveCount = (int)group.PrimitiveCount,
                        };
                    }
                    else
                    {
                        renderGroup = new RenderGroup(graphicsSettings)
                        {
                            MinVertexIndex = 0,
                            VerticesCount = vState.Count,
                            StartIndex = 0,
                            PrimitiveCount = ((int)indices.Tag) / 3,
                        };
                    }

                    renderGroup.VertexState = vState;
                    renderGroup.Indices = indices;
                    renderGroup.RawVertices = rawVertices;
                    renderGroup.RawIndices = rawIndices;

                    if (group.Material.ShowArmor)
                    {
                        renderGroup.RenderArmor = true;
                        renderGroup.Textures = null;
                        renderGroup.ArmorGroup = group.Material.Armor;

                    }
                    else
                    {
                        renderGroup.RenderArmor = false;

                        var textures = new Dictionary<string, Texture>();

                        foreach (var property in group.Material.Properties)
                        {
                            var texturePath = property.Texture;

                            if (string.IsNullOrWhiteSpace(texturePath))
                            {
                                if (property.Name == "alphaTestEnable" && group.Material.Fx != "shaders/std_effects/PBS_tank.fx")
                                {
                                    renderGroup.AlphaTestEnable = property.BoolValue;
                                }
                                else if (property.Name == "alphaReference")
                                {
                                    renderGroup.AlphaReference = property.IntValue;
                                }
                                else if (property.Name == "g_useNormalPackDXT1")
                                {
                                    renderGroup.UseNormalPackDXT1 = property.BoolValue;
                                }
                                else if (property.Name == "g_detailPower")
                                {
                                    renderGroup.DetailPower = property.FloatValue;
                                }
                                else if (property.Name == "g_metallicDetailUVTiling")
                                {
                                    renderGroup.DetailUVTiling = property.Vector4Value;
                                }
                            }
                            else
                            {
                                try
                                {
                                    using (var stream = ModuleMesh.OpenTexture(fileLocator, texturePath))
                                    {
                                        var info = ImageInformation.FromStream(stream, true);
                                        var texture = Texture.FromStream(device, stream);
                                        textures[property.Name] = texture;

                                        if (property.Name == "normalMap" && !renderGroup.UseNormalBC1)
                                        {
                                            if (info.Format == Format.Dxt1)
                                            {
                                                renderGroup.UseNormalBC1 = true;
                                            }
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    this.LogInfo("can't load texture {0} for {1}", texturePath, property.Name);
                                }
                            }
                        }

                        renderGroup.Textures = textures;
                    }

                    _renderGroups.Add(renderGroup);
                }
            }
        }


        public void Render(Effect effect, ref int triangleCount)
        {
            if (Monitor.TryEnter(this))
            {
                try
                {
                    //_device.SetRenderState(RenderState.MultisampleAntialias, true);

                    foreach (var group in _renderGroups)
                    {
                        group.ApplyState(_device, effect);

                        triangleCount += group.PrimitiveCount;

                        effect.Begin();
                        effect.BeginPass(0);

                        _device.DrawIndexedPrimitive(PrimitiveType.TriangleList,
                                                     0,
                                                     group.MinVertexIndex,
                                                     group.VerticesCount,
                                                     group.StartIndex,
                                                     group.PrimitiveCount);

                        effect.EndPass();
                        effect.End();

                    }
                }
                finally
                {
                    Monitor.Exit(this);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {

            Monitor.Enter(this);
            try
            {
                if (disposing)
                {
                    foreach (var group in _renderGroups)
                    {
                        group.Indices.Dispose();
                        group.VertexState.VertexBuffer.Dispose();
                        group.VertexState.VertexDeclaration.Dispose();

                        if (group.Textures != null)
                        {
                            foreach (var texture in group.Textures)
                            {
                                texture.Value.Dispose();
                            }
                        }
                    }
                }
                _renderGroups.Clear();
            }
            finally
            {
                Monitor.Exit(this);
            }

            base.Dispose(disposing);
        }


        private static IndexBuffer ConvertToIndexBuffer(IList<int> nlist, Device device)
        {
            var indicesBuffer = new IndexBuffer(device, sizeof(int) * nlist.Count, Usage.WriteOnly, Pool.Default, false);
            var data = indicesBuffer.Lock(0, 0, LockFlags.None);

            foreach (var n in nlist)
            {
                data.Write(n);
            }
            indicesBuffer.Unlock();
            indicesBuffer.Tag = nlist.Count;
            return indicesBuffer;
        }


        private static VertexState ConvertToVertexBuffer(ModelPrimitive.VerticesList vlist, Device device)
        {
            var vertexElems = new[] {
                new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                new VertexElement(0, 12, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
                new VertexElement(0, 24, DeclarationType.Float2,DeclarationMethod.Default,DeclarationUsage.TextureCoordinate,0),
                new VertexElement(0, 32, DeclarationType.Float3,DeclarationMethod.Default,DeclarationUsage.Tangent,0),
                new VertexElement(0, 44, DeclarationType.Float3,DeclarationMethod.Default,DeclarationUsage.Binormal,0),
                VertexElement.VertexDeclarationEnd
            };

            var vertexSize = 56;

            var vertexBuffer = new VertexBuffer(device, vertexSize * vlist.Vertices.Count, Usage.WriteOnly, VertexFormat.None, Pool.Default);

            var data = vertexBuffer.Lock(0, 0, LockFlags.None);
            foreach (var v in vlist.Vertices)
            {
                data.Write(DXUtils.Convert(v.Position));
                data.Write(DXUtils.Convert(v.Normal));
                data.Write(DXUtils.Convert(v.TextureCoordinates));
                data.Write(DXUtils.Convert(v.Tangent));
                data.Write(DXUtils.Convert(v.Binormal));
            }

            var vertexDecl = new VertexDeclaration(device, vertexElems);

            vertexBuffer.Unlock();

            return new VertexState() { VertexBuffer = vertexBuffer, VertexDeclaration = vertexDecl, Stride = vertexSize, Count = vlist.Vertices.Count };
        }

        private static Stream OpenTexture(IFileLocator fileLocator, string path)
        {
            Stream stream;
            if (fileLocator.TryOpenRead(path, out stream))
                return stream;

            if (Path.GetExtension(path) == ".tga")
            {
                path = Path.ChangeExtension(path, ".dds");
                return fileLocator.OpenRead(path);
            }

            throw new FileNotFoundException("file not found", path);
        }
    }
}

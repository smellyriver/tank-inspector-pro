using System;
using System.Linq;
using SharpDX;
using SharpDX.Direct3D9;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Graphics.Frameworks;
using Smellyriver.TankInspector.Pro.IO;

namespace Smellyriver.TankInspector.Pro.Graphics.Scene
{
    partial class HangarScene
    {
        class ModuleModelRenderer : IDisposable
        {

            public Matrix Transform { get; set; }

            private ModuleModel _model;

            public ModuleModel Model
            {
                get { return _model; }
                set
                {
                    _model = value;
                    this.LoadModel();
                }
            }

            private IFileLocator _fileLocator;

            public IFileLocator FileLocator
            {
                get { return _fileLocator; }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException();

                    _fileLocator = value;
                    this.LoadModel();
                }
            }

            private ModelType? _modelType;

            public ModelType? ModelType
            {
                get { return _modelType; }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException();

                    _modelType = value;
                    this.LoadModel();
                }
            }


            private readonly HangarScene _owner;

            public ModuleModelRenderer(HangarScene owner)
            {
                _owner = owner;
            }

            private void LoadModel()
            {
                if (this.Model != null)
                {
                    if (this.FileLocator == null || this.ModelType == null)
                        return;

                    this.Model.Load(this.ModelType.Value, this.FileLocator, _owner.Renderer.Device, _owner.GraphicsSettings);
                }
            }

            public void Render(Effect effect, ref int triangleCount)
            {
                this.Model.Render(effect, ref triangleCount);
            }

            public void HitTest(Ray ray, ref CollisionModelHitTestResult result)
            {
                if (this.Model.Mesh == null)
                    return;

                if (this.Model.Mesh.Groups == null)
                    return;

                var invTrans = Matrix.Invert(this.Transform);
                ray.Position = invTrans.TransformCoord(ray.Position);
                ray.Direction = invTrans.TransformNormal(ray.Direction);
                ray.Direction.Normalize();

                var collisionGroup = this.Model.Mesh.Groups.ToArray();

                foreach (var group in collisionGroup)
                {
                    foreach (var theTriangle in group.Triangles)
                    {
                        var triangle = theTriangle;
                        float distance;
                        if (ray.Intersects(ref triangle.v1, ref triangle.v2, ref triangle.v3, out distance))
                        {
                            var normal = HangarScene.CalculateNormal(ref triangle.v1, ref triangle.v2, ref triangle.v3);
                            normal.Normalize();
                            var dot = Math.Abs(Vector3.Dot(ray.Direction, -normal));
                            result.Hits.Add(new CollisionModelHit()
                            {
                                Distance = distance,
                                Armor = group.ArmorGroup,
                                InjectionCosine = dot,
                                Mesh = this.Model.Mesh
                            });
                        }
                    }
                }
            }

            public void Dispose()
            {
                Disposer.RemoveAndDispose(ref _model);
            }
        }
    }
}

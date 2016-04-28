using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.Direct3D9;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Graphics.Frameworks;

namespace Smellyriver.TankInspector.Pro.Graphics.Scene
{
    class ProjectileTracer : SceneMeshBase
    {

        struct TracerLineVertex
        {
            public Vector3 Position;
            public float Penetration;
        }
        List<ModuleMesh> _hitMeshs;
        List<TracerLineVertex> _tracerLines;
        Device _device;
        VertexDeclaration _vertexDeclaration;
        Ray _tracerRay;
        private float _traceLengthSquared;
        private IOrderedEnumerable<CollisionModelHit> _orderedHits;

        public Ray TracerRay
        {
            get { return _tracerRay; }
        }

        public override void Reset(DrawEventArgs args)
        {

        }

        public void Render()
        {
            _device.VertexDeclaration = _vertexDeclaration;
            var vertices = _tracerLines.ToArray();
            _device.DrawUserPrimitives(PrimitiveType.LineList, 0, vertices.Length / 2, vertices);
        }

        protected override void Dispose(bool disposing)
        {
            this.LogInfo("dispose");
            Disposer.RemoveAndDispose(ref _vertexDeclaration);
        }

        public bool Hit(ModuleMesh tankMesh)
        {
            return _hitMeshs.Contains(tankMesh);
        }

        public void Refresh(TestShellInfo testShell)
        {
            this.LogInfo("refresh");
            _tracerLines = new List<TracerLineVertex>();
            _tracerLines.Add(new TracerLineVertex() { Position = _tracerRay.Position, Penetration = 0 });

            PenetrationState penetrationState = PenetrationState.NotApplicable;

            bool mayRicochet = true;

            float equivalentThickness = 0.0f;
            float heatExplodedDistance = 0.0f;
            bool heatExploded = false;

            foreach (var hit in _orderedHits)
            {
                _hitMeshs.Add(hit.Mesh);

                if (!heatExploded)
                {
                    _tracerLines.Add(new TracerLineVertex() { Position = _tracerRay.Position + hit.Distance * _tracerRay.Direction, Penetration = equivalentThickness });
                }

                var thisImpactAngle = DXUtils.ConvertRadiansToDegrees(Math.Acos(hit.InjectionCosine));

                if (hit.Armor.UseHitAngle && hit.Armor.Thickness != 0.0)
                {
                    mayRicochet = mayRicochet && hit.Armor.MayRicochet;

                    double thisNomarlizationAngle = testShell.ShellType.GetBaseNormalization();
                    if (testShell.Caliber >= hit.Armor.Thickness * 3.0)
                    {
                        mayRicochet = false;
                        thisNomarlizationAngle *= 1.4 * testShell.Caliber / hit.Armor.Thickness;
                    }
                    else if (testShell.Caliber >= hit.Armor.Thickness * 2.0)
                    {
                        thisNomarlizationAngle *= 1.4 * testShell.Caliber / hit.Armor.Thickness;
                    }

                    if (mayRicochet && thisImpactAngle > testShell.ShellType.GetRicochetAngle())
                    {
                        penetrationState = PenetrationState.Richochet;
                        break;
                    }

                    if (testShell.ShellType.HasNormalizationEffect())
                    {
                        if (thisNomarlizationAngle > thisImpactAngle)
                            thisNomarlizationAngle = thisImpactAngle;

                        thisImpactAngle -= thisNomarlizationAngle;
                    }

                    if (!heatExploded)
                    {
                        equivalentThickness += (float)(hit.Armor.Thickness / Math.Cos(DXUtils.ConvertDegreesToRadians(thisImpactAngle)));
                    }
                    else
                    {
                        var distance = (hit.Distance - heatExplodedDistance);

                        var attenuation = 1 - distance * 0.5;
                        if (attenuation < 0)
                        {
                            _tracerLines.Add(new TracerLineVertex() { Position = _tracerRay.Position + hit.Distance * _tracerRay.Direction, Penetration = 0 });
                            penetrationState = PenetrationState.Unpenetratable;
                            break;
                        }

                        equivalentThickness += (float)(hit.Armor.Thickness / attenuation / Math.Cos(DXUtils.ConvertDegreesToRadians(thisImpactAngle)));

                        _tracerLines.Add(new TracerLineVertex() { Position = _tracerRay.Position + hit.Distance * _tracerRay.Direction, Penetration = equivalentThickness });
                    }

                }
                else
                {
                    if (!heatExploded)
                    {
                        equivalentThickness += (float)hit.Armor.Thickness;
                    }
                    else
                    {
                        var distance = (hit.Distance - heatExplodedDistance);

                        var attenuation = 1 - distance * 0.5;
                        if (attenuation < 0)
                        {
                            _tracerLines.Add(new TracerLineVertex() { Position = _tracerRay.Position + hit.Distance * _tracerRay.Direction, Penetration = 0 });
                            penetrationState = PenetrationState.Unpenetratable;
                            break;
                        }

                        equivalentThickness += (float)(hit.Armor.Thickness / attenuation);

                        _tracerLines.Add(new TracerLineVertex() { Position = _tracerRay.Position + hit.Distance * _tracerRay.Direction, Penetration = equivalentThickness });
                    }
                }



                if (!hit.Armor.IsSpacedArmor)
                {
                    penetrationState = PenetrationState.Penetratable;
                    break;
                }
                else
                {
                    if (testShell.ShellType == ShellType.HE || testShell.ShellType == ShellType.PremiumHE)
                    {
                        penetrationState = PenetrationState.Unpenetratable;
                        break;
                    }
                    else if (testShell.ShellType == ShellType.HEAT && !heatExploded)
                    {
                        heatExploded = true;
                        heatExplodedDistance = hit.Distance;
                        mayRicochet = false;
                    }
                }

                _tracerLines.Add(new TracerLineVertex() { Position = _tracerRay.Position + hit.Distance * _tracerRay.Direction, Penetration = equivalentThickness });
            }


            if (penetrationState == PenetrationState.Penetratable)
            {
                for (int i = 0; i != _tracerLines.Count; ++i)
                {
                    _tracerLines[i] = new TracerLineVertex() { Position = _tracerLines[i].Position, Penetration = (equivalentThickness - _tracerLines[i].Penetration) * 0.5f };
                }
            }
            else if (penetrationState == PenetrationState.Richochet
                || penetrationState == PenetrationState.Unpenetratable
                || penetrationState == PenetrationState.NotApplicable)
            {
                for (int i = 0; i != _tracerLines.Count; ++i)
                {
                    _tracerLines[i] = new TracerLineVertex() { Position = _tracerLines[i].Position, Penetration = -1.0f };
                }
            }

            BuildCollision();
        }

        public ProjectileTracer(Device device,Ray hitRay, IOrderedEnumerable<CollisionModelHit> orderedHits, TestShellInfo testShell)
        {
            this.LogInfo("new projectile tracer");

            _tracerRay = hitRay;
            _hitMeshs = new List<ModuleMesh>();

            _device = device;
            _vertexDeclaration = new VertexDeclaration(_device, new[]
                {
                    new VertexElement(0,0,DeclarationType.Float3,DeclarationMethod.Default,DeclarationUsage.Position,0),
                    new VertexElement(0,12,DeclarationType.Float1,DeclarationMethod.Default,DeclarationUsage.Color,0),
                    VertexElement.VertexDeclarationEnd,
                });

            _orderedHits = orderedHits;

            Refresh(testShell); 
        }

        private void BuildCollision()
        {
            if (_tracerLines.Count > 1)
            {
                _traceLengthSquared = (_tracerRay.Position - _tracerLines[_tracerLines.Count - 1].Position).LengthSquared();
            }
        }

        public static bool RayIntersectsRay(ref Ray ray1, ref Ray ray2, out Vector3 point)
        {
            Vector3 directionDelta;
            Vector3.Cross(ref ray1.Direction, ref ray2.Direction, out directionDelta);
            float directionDeltaLength = directionDelta.Length();
            if (((Math.Abs(directionDeltaLength) < 1E-06f) && (Math.Abs((float)(ray2.Position.X - ray1.Position.X)) < 1E-06f)) && ((Math.Abs((float)(ray2.Position.Y - ray1.Position.Y)) < 1E-06f) && (Math.Abs((float)(ray2.Position.Z - ray1.Position.Z)) < 1E-06f)))
            {
                point = Vector3.Zero;
                return true;
            }
            directionDeltaLength *= directionDeltaLength;
            float xpDelta = ray2.Position.X - ray1.Position.X;
            float ypDelta = ray2.Position.Y - ray1.Position.Y;
            float zpDelta = ray2.Position.Z - ray1.Position.Z;
            float d2x = ray2.Direction.X;
            float d2y = ray2.Direction.Y;
            float d2z = ray2.Direction.Z;
            float ddx = directionDelta.X;
            float ddy = directionDelta.Y;
            float ddz = directionDelta.Z;
            float cross1 = ((((((xpDelta * d2y) * ddz) + ((ypDelta * d2z) * ddx)) + ((zpDelta * d2x) * ddy)) - ((xpDelta * d2z) * ddy)) - ((ypDelta * d2x) * ddz)) - ((zpDelta * d2y) * ddx);
            d2x = ray1.Direction.X;
            d2y = ray1.Direction.Y;
            d2z = ray1.Direction.Z;
            float cross2 = ((((((xpDelta * d2y) * ddz) + ((ypDelta * d2z) * ddx)) + ((zpDelta * d2x) * ddy)) - ((xpDelta * d2z) * ddy)) - ((ypDelta * d2x) * ddz)) - ((zpDelta * d2y) * ddx);
            float dir1 = cross1 / directionDeltaLength;
            float dir2 = cross2 / directionDeltaLength;
            Vector3 point1 = ray1.Position + ((Vector3)(dir1 * ray1.Direction));
            Vector3 point2 = ray2.Position + ((Vector3)(dir2 * ray2.Direction));
            if (((Math.Abs((float)(point2.X - point1.X)) > 0.05) || (Math.Abs((float)(point2.Y - point1.Y)) > 0.05)) || (Math.Abs((float)(point2.Z - point1.Z)) > 0.05))
            {
                point = Vector3.Zero;
                return false;
            }
            point = point1;
            return true;
        }
        

        public void HitTest(Ray ray,ref ProjectileTracerHitTestResult result)
        {
            Vector3 point;
            if (_tracerRay.Position == ray.Position)
            {
                return;
            }

            if (ProjectileTracer.RayIntersectsRay(ref _tracerRay, ref ray, out point))
            {
                if ((_tracerRay.Position - point).LengthSquared() < _traceLengthSquared)
                {
                    var distance = (ray.Position - point).Length();
                    result.ProjectileTracerHits.Add(new ProjectileTracerHit() { Tracer = this, Distance = distance });
                }
            }
        }
    }
}

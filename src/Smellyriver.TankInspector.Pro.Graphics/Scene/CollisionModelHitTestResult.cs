using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Graphics.Frameworks;

namespace Smellyriver.TankInspector.Pro.Graphics.Scene
{

    class CollisionModelHitTestResult
    {
        public List<CollisionModelHit> Hits = new List<CollisionModelHit>();
        private Ray _hitRay;

        public CollisionModelHitTestResult(Ray ray)
        {
            this._hitRay = ray;
        }

        public Ray HitRay { get { return _hitRay; } }

        public CollisionModelHit? ClosesetSpacingArmorHit
        {
            get
            {
                if (Hits.Any(a => a.Armor.IsSpacedArmor))
                    return Hits.Where(a => a.Armor.IsSpacedArmor).Aggregate((a, b) => a.Distance > b.Distance ? b : a);
                return null;
            }
        }

        public CollisionModelHit? ClosesetRegularArmorHit
        {
            get
            {
                if (Hits.Any(a => !a.Armor.IsSpacedArmor))
                    return Hits.Where(a => !a.Armor.IsSpacedArmor).Aggregate((a, b) => a.Distance > b.Distance ? b : a);
                return null;
            }
        }

        public CollisionModelHit? ClosesetArmorHit
        {
            get
            {
                if (Hits.Count == 0) return null;
                return Hits.Aggregate((a, b) => a.Distance > b.Distance ? b : a);
            }
        }

        public float? EquivalentThickness
        {
            get
            {
                float thickness = 0.0f;

                var orderedHits = Hits.OrderBy((h) => h.Distance);

                foreach (var hit in orderedHits)
                {
                    if (hit.Armor.UseHitAngle)
                    {
                        thickness += (float)hit.Armor.Thickness / hit.InjectionCosine;
                    }
                    else
                    {
                        thickness += (float)hit.Armor.Thickness;
                    }

                    if (!hit.Armor.IsSpacedArmor)
                    {
                        return thickness;
                    }
                }
                return null;
            }
        }

        public IOrderedEnumerable<CollisionModelHit> OrderedHits
        {
            get { return Hits.OrderBy((h) => h.Distance); }
        }

        public ShootTestResult GetShootTestResult(TestShellInfo testShell)
        {
            PenetrationState penetrationState = PenetrationState.NotApplicable;
            double equivalentThickness = 0.0;
            double impactAngle = 0.0;
            double nomarlizationAngle = 0.0;
            bool is2x = false;
            bool is3x = false;
            bool mayRicochet = true;

            bool heatExploded = false;
            double heatExplodedDistance = 0.0;

            bool isFirstHit = true;
            var orderedHits = Hits.OrderBy((h) => h.Distance);

            foreach (var hit in orderedHits)
            {
                var thisImpactAngle = DXUtils.ConvertRadiansToDegrees(Math.Acos(hit.InjectionCosine));

                if (isFirstHit)
                {
                    impactAngle = thisImpactAngle;
                }

                if (hit.Armor.UseHitAngle && hit.Armor.Thickness != 0.0)
                {
                    mayRicochet = mayRicochet && hit.Armor.MayRicochet;

                    double thisNomarlizationAngle = testShell.ShellType.GetBaseNormalization();

                    if (testShell.Caliber >= hit.Armor.Thickness * 3.0 && testShell.ShellType.HasNormalizationEffect())
                    {
                        is3x = true;
                        is2x = true;
                        mayRicochet = false;
                        thisNomarlizationAngle *= 1.4 * testShell.Caliber / hit.Armor.Thickness;
                    }
                    else if (testShell.Caliber >= hit.Armor.Thickness * 2.0 && testShell.ShellType.HasNormalizationEffect())
                    {

                        is3x = false;
                        is2x = testShell.ShellType.HasNormalizationEffect();
                        thisNomarlizationAngle *= 1.4 * testShell.Caliber / hit.Armor.Thickness;
                    }

                    if (mayRicochet && thisImpactAngle > testShell.ShellType.GetRicochetAngle())
                    {
                        penetrationState = PenetrationState.Richochet;
                        break;
                    }

                    if (isFirstHit && testShell.ShellType.HasNormalizationEffect())
                    {
                        if (thisNomarlizationAngle > thisImpactAngle)
                            thisNomarlizationAngle = thisImpactAngle;

                        thisImpactAngle -= thisNomarlizationAngle;

                        nomarlizationAngle = thisNomarlizationAngle;
                    }

                    if (!heatExploded)
                    {
                        equivalentThickness += hit.Armor.Thickness / Math.Cos(DXUtils.ConvertDegreesToRadians(thisImpactAngle));
                    }
                    else
                    {
                        var distance = (hit.Distance - heatExplodedDistance);

                        var attenuation = 1 - distance * 0.5;
                        if (attenuation < 0)
                        {
                            penetrationState = PenetrationState.Unpenetratable;
                            break;
                        }

                        equivalentThickness += hit.Armor.Thickness / attenuation / Math.Cos(DXUtils.ConvertDegreesToRadians(thisImpactAngle));
                    }
                }
                else
                {
                    if (!heatExploded)
                    {
                        equivalentThickness += hit.Armor.Thickness;
                    }
                    else
                    {
                        var distance = (hit.Distance - heatExplodedDistance);

                        var attenuation = 1 - distance * 0.5;
                        if (attenuation < 0)
                        {
                            penetrationState = PenetrationState.Unpenetratable;
                            break;
                        }

                        equivalentThickness += hit.Armor.Thickness / attenuation;
                    }
                }

                if (!hit.Armor.IsSpacedArmor)
                {
                    if (equivalentThickness < 999.0)
                        penetrationState = PenetrationState.Penetratable;
                    else
                        penetrationState = PenetrationState.Unpenetratable;

                    break;
                }
                else
                {
                    if (testShell.ShellType == ShellType.HE || testShell.ShellType == ShellType.PremiumHE)
                    {
                        penetrationState = PenetrationState.Unpenetratable;
                        break;
                    }
                    else if (testShell.ShellType == ShellType.HEAT && isFirstHit)
                    {
                        heatExploded = true;
                        heatExplodedDistance = hit.Distance;
                        mayRicochet = false;
                    }
                }
                isFirstHit = false;
            }
            return new ShootTestResult(penetrationState, equivalentThickness, impactAngle, nomarlizationAngle, is2x, is3x);
        }

        
    }
}

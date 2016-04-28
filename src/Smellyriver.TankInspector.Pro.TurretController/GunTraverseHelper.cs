using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Common.Utilities;

namespace Smellyriver.TankInspector.Pro.TurretController
{
    internal static class GunTraverseHelper
    {
        public static Point PolarToCartesian(Point center, double radius, double degrees)
        {
            return new Point(radius * Math.Cos(degrees * Math.PI / 180) + center.X, -radius * Math.Sin(degrees * Math.PI / 180) + center.Y);
        }

        public static void CartesianToPolar(Point center, Point point, out double radius, out double degree)
        {
            var d = point - center;
            radius = d.Length;
            degree = GunTraverseHelper.NormalizeAngle(Math.Atan2(d.Y, d.X) * 180 / Math.PI);
        }

        public static double NormalizeAngle(double degree, double minimum = 0)
        {
            while (degree < minimum)
                degree += 360;

            while (degree > minimum + 360)
                degree -= 360;

            return degree;
        }

        public static double RotateAngle(double from, double to, double delta, double limitRight, double limitLeft)
        {
            from = GunTraverseHelper.NormalizeAngle(from);
            to = GunTraverseHelper.NormalizeAngle(to);

            if (Math.Abs(from - to) < 0.0001)
                return to;

            delta = GunTraverseHelper.NormalizeAngle(delta);
            double direction;

            if (from < to)
            {
                if (limitRight + limitLeft < 360
                    && from <= limitRight
                    && to >= 360 - limitLeft)
                    direction = -1.0;
                else if (to - from > 180)
                    direction = -1.0;
                else
                    direction = 1.0;
            }
            else
            {
                if (limitRight + limitLeft < 360
                    && to <= limitRight
                    && from >= 360 - limitLeft)
                    direction = 1.0;
                else if (from - to > 180)
                    direction = 1.0;
                else
                    direction = -1.0;
            }

            var result = from + delta * direction;
            if ((from < to && result > to) || (from > to && result < to))
                result = to;

            return result;
        }

        private static double DefaultVerticalTraverseTransform(double value)
        {
            return value;
        }


        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public static Geometry CreateGeometry(GunPitchLimitsComponent traverse,
                                              TurretYawLimits yawLimits,
                                              Point center,
                                              Func<double, double> radiusConverter,
                                              double geometryRotation = 0)
        {
            Geometry geometry;
            double maxRadius;
            var data = traverse.Data;

            if (traverse.HasSingularValue)
            {
                var radius = radiusConverter(data[0].Limit);
                geometry = new EllipseGeometry(center, radius, radius);
                maxRadius = radius;
            }
            else
            {
                maxRadius = radiusConverter(traverse.GetMaxValue());

                var figureFigure = new PathFigure();

                var previousTheta = 0.0;
                var previousRadius = 0.0;
                var previousNode = (GunPitchLimitsComponent.PitchData)null;

                for (var i = 0; i < data.Length; ++i)
                {
                    var node = data[i];
                    var theta = geometryRotation + node.Angle * 360;
                    var radius = radiusConverter(node.Limit);
                    var point = GunTraverseHelper.PolarToCartesian(center, radius, theta);
                    var thetaRange = theta - previousTheta;

                    if (i == 0)
                        figureFigure.StartPoint = point;
                    else
                    {
                        Debug.Assert(previousNode != null, "previousNode != null");

                        if (previousNode.Angle == node.Angle)
                            figureFigure.Segments.Add(new LineSegment(point, true));
                        else if (previousNode.Limit == node.Limit)
                        {

                            figureFigure.Segments.Add(new ArcSegment(point,
                                                                     new Size(radius, radius),
                                                                     thetaRange,
                                                                     thetaRange >= 180,
                                                                     SweepDirection.Counterclockwise,
                                                                     true));
                        }
                        else
                        {
                            figureFigure.Segments.Add(GunTraverseHelper.CreateTraverseFigureTransition(ref figureFigure,
                                                                                                       ref center,
                                                                                                       previousTheta,
                                                                                                       thetaRange,
                                                                                                       previousRadius,
                                                                                                       radius));
                        }
                    }

                    previousTheta = theta;
                    previousNode = node;
                    previousRadius = radius;
                }


                var pathGeometry = new PathGeometry();
                pathGeometry.Figures.Add(figureFigure);
                geometry = pathGeometry;
            }

            return GunTraverseHelper.ProcessGeometryHorizontalTraverse(traverse,
                                                                       yawLimits,
                                                                       center,
                                                                       geometryRotation,
                                                                       geometry,
                                                                       maxRadius);
        }

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        private static Geometry ProcessGeometryHorizontalTraverse(GunPitchLimitsComponent traverse,
                                                                  TurretYawLimits yawLimits,
                                                                  Point center,
                                                                  double geometryRotation,
                                                                  Geometry geometry,
                                                                  double maxRadius)
        {

            if (yawLimits == null || yawLimits.Range == 360)
                return geometry;
            else
            {
                // create a horizontal fan
                var fanFigure = new PathFigure { StartPoint = center };
                var point = GunTraverseHelper.PolarToCartesian(center, maxRadius, 360 - yawLimits.Right + geometryRotation);
                fanFigure.Segments.Add(new LineSegment(point, true));
                point = GunTraverseHelper.PolarToCartesian(center, maxRadius, -yawLimits.Left + geometryRotation);
                fanFigure.Segments.Add(new ArcSegment(point,
                                                      new Size(maxRadius, maxRadius),
                                                      yawLimits.Range,
                                                      yawLimits.Range > 180,
                                                      SweepDirection.Counterclockwise,
                                                      true));
                fanFigure.IsClosed = true;
                var fanGeometry = new PathGeometry();
                fanGeometry.Figures.Add(fanFigure);

                return new CombinedGeometry(GeometryCombineMode.Intersect, geometry, fanGeometry);
            }
        }


        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public static Geometry CreateGeometry(GunPitchLimitsComponent traverse,
                                              TurretYawLimits yawLimits,
                                              double size,
                                              Point center,
                                              double geometryRotation = 0,
                                              double margin = 0,
                                              double padding = 0,
                                              Func<double, double> verticalTraverseTransform = null)
        {
            if (verticalTraverseTransform == null)
                verticalTraverseTransform = GunTraverseHelper.DefaultVerticalTraverseTransform;

            if (traverse.HasSingularValue && yawLimits.Range == 360)
            {
                var radius = size / 2 - margin;
                return new EllipseGeometry(center, radius, radius);
            }
            else
            {
                var maxRadiusInDegrees = traverse.GetMaxValue();
                var scale = (size / 2 - margin - padding) / maxRadiusInDegrees;

                Func<double, double> radiusConverter = r => padding + Math.Max(verticalTraverseTransform(r) * scale, 0);

                return GunTraverseHelper.CreateGeometry(traverse, yawLimits, center, radiusConverter, geometryRotation);
            }
        }

        private static PathSegment CreateTraverseFigureTransition(ref PathFigure figure, ref Point center, double startAngle, double transitionAngle, double from, double to)
        {
            const double step = 0.1;
            var endAngle = startAngle + transitionAngle;
            var delta = to - from;
            var segment = new PolyLineSegment();
            segment.IsStroked = true;
            for (var angle = startAngle + step; angle <= endAngle; angle += step)
            {
                var traverse = from + delta * (angle - startAngle) / transitionAngle;
                segment.Points.Add(GunTraverseHelper.PolarToCartesian(center, traverse, angle));
            }

            return segment;
        }

    }
}

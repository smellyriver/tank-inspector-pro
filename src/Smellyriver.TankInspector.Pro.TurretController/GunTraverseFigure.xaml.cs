using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Smellyriver.TankInspector.Pro.Data.Entities;

namespace Smellyriver.TankInspector.Pro.TurretController
{
    public partial class GunTraverseFigure : UserControl
    {
        private static string FormatAngle(double angle)
        {
            //todo: localization
            return string.Format("{0} deg", angle.ToString("#,0.#"));
        }

        public GunTraverseFigure()
        {
            InitializeComponent();
        }

        public void SetTraverse(GunPitchLimitsComponent gunPitch, TurretYawLimits turretYaw)
        {
            this.CurveCanvas.Children.Clear();

            if (gunPitch == null)
                return;

            var traverseFigureStrokeStyle = this.FindResource("TraverseFigure") as Style;

            var maxRadius = gunPitch.GetMaxValue();
            //maxRadius = Math.Ceiling(maxRadius / 5) * 5;
            const double margin = 1;

            var scale = 80 / (maxRadius + margin);
            var size = scale * maxRadius * 2;
            var center = new Point(100, 100);
            var figurePath = new Path
            {
                Data = GunTraverseHelper.CreateGeometry(gunPitch,
                                                        turretYaw,
                                                        size,
                                                        center,
                                                        90,
                                                        scale * margin,
                                                        verticalTraverseTransform: v => v + 1),
                Style = traverseFigureStrokeStyle
            };

            this.CurveCanvas.Children.Add(figurePath);

            // draw references
            var thinReferenceGeometry = new GeometryGroup();
            var thickReferenceGeometry = new GeometryGroup();

            const double referenceCircleGap = 20;
            const double halfReferenceCircleGap = referenceCircleGap / 2;

            var sign = Math.Sign(maxRadius);
            var absMaxRadius = Math.Abs(maxRadius);

            var thickDivisor = absMaxRadius > 45 ? 20 : absMaxRadius > 25 ? 10 : 5;
            var thinDivisor = absMaxRadius > 25 ? 5 : 1;

            for (var i = thinDivisor; i <= absMaxRadius; i += thinDivisor)
            {
                var radius = scale * i * sign;

                Geometry geometry;

                var isThickRing = i % thickDivisor == 0;
                if (absMaxRadius < 10 || isThickRing)
                {
                    var gapHalfAngle = Math.Asin(halfReferenceCircleGap / radius) * 180 / Math.PI;
                    if (double.IsNaN(gapHalfAngle))
                        gapHalfAngle = 180;
                    var startPoint = GunTraverseHelper.PolarToCartesian(center, radius, gapHalfAngle);
                    var endPoint = GunTraverseHelper.PolarToCartesian(center, radius, 360 - gapHalfAngle);
                    var arc = new ArcSegment(endPoint, new Size(radius, radius), 360 - gapHalfAngle * 2, true, SweepDirection.Counterclockwise, true);
                    var figure = new PathFigure(startPoint, new[] { arc }, false);
                    geometry = new PathGeometry(new[] { figure });


                    var referenceTextContainer = new Grid
                    {
                        Width = 20,
                        Height = referenceCircleGap
                    };
                    var referenceTextContainerPosition = GunTraverseHelper.PolarToCartesian(center, radius, 0);
                    Canvas.SetLeft(referenceTextContainer, referenceTextContainerPosition.X - 10);
                    Canvas.SetTop(referenceTextContainer, referenceTextContainerPosition.Y - halfReferenceCircleGap);

                    var referenceText = new TextBlock
                    {
                        Text = (i * sign).ToString(),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = isThickRing ? 10 : 8,
                        Foreground = isThickRing ? Brushes.White : new SolidColorBrush(Color.FromArgb(0x80, 0xff, 0xff, 0xff))
                    };


                    referenceTextContainer.Children.Add(referenceText);

                    this.CurveCanvas.Children.Add(referenceTextContainer);
                }
                else
                {
                    geometry = new EllipseGeometry(center, radius, radius);
                }

                if (isThickRing)
                    thickReferenceGeometry.Children.Add(geometry);
                else
                    thinReferenceGeometry.Children.Add(geometry);
            }

            var thinReferencePath = new Path
            {
                Data = thinReferenceGeometry,
                Style = this.FindResource("ThinReferenceStroke") as Style
            };
            this.CurveCanvas.Children.Add(thinReferencePath);

            var thickReferencePath = new Path
            {
                Data = thickReferenceGeometry,
                Style = this.FindResource("ThickReferenceStroke") as Style
            };
            this.CurveCanvas.Children.Add(thickReferencePath);

        }

    }
}
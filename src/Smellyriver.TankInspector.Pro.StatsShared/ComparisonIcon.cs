using System.Collections.Generic;
using System.Windows.Media;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.Pro.Data.Stats;

namespace Smellyriver.TankInspector.Pro.StatsShared
{
    public static class ComparisonIcon
    {
        private static readonly Dictionary<string, ImageSource> s_benchmarkIcons
            = new Dictionary<string, ImageSource>();

        public static ImageSource GetIcon(IStat stat, string originalValue, string changedValue)
        {
            if (stat.CompareStrategy == CompareStrategy.NotComparable)
                return null;

            double? difference;

            if (stat.BenchmarkThreshold.Type == BenchmarkThresholdType.Relative)
                difference = stat.GetDifferenceRatio(changedValue, originalValue);
            else
                difference = stat.GetDifference(changedValue, originalValue);

            if (difference == null)
                return null;

            var imageName = "Comparison";
            if (stat.CompareStrategy == CompareStrategy.Plain)
                imageName += "Plain";

            var differenceLevel = ((int)(difference.Value / stat.BenchmarkThreshold.Value * 4)).Clamp(-3, 3);

            imageName += differenceLevel.ToString();

            return s_benchmarkIcons.GetOrCreate(imageName,
                () =>
                {
                    var image = (ImageSource)BitmapImageEx.LoadAsFrozen(string.Format("Resources/Images/{0}_16.png", imageName));
                    return image;
                });
        }


        //private static ImageSource CreateBenchmarkIcon(double differenceLevel)
        //{
        //    if (differenceLevel == 0)
        //        return null;

        //    const double maxHeight = 22;
        //    const double width = 5;
        //    const double horizontalCenter = maxHeight / 2;
        //    const double verticalCenter = width / 2;

        //    var height = maxHeight / 2 * Math.Abs(differenceLevel);

        //    Color color;
        //    if (this.Model.CompareStrategy == CompareStrategy.Plain)
        //        color = Color.FromArgb(0xff, 0x70, 0xb1, 0xcf);
        //    else
        //    {
        //        if (differenceLevel > 0)
        //            color = Color.FromArgb(0xff, 0x38, 0xcc, 0x00);
        //        else
        //            color = Color.FromArgb(0xff, 0xf5, 0x08, 0x00);
        //    }

        //    var brush = new SolidColorBrush(color);

        //    var figure = new PathFigure();
        //    figure.StartPoint = new Point(0, horizontalCenter);
        //    if (differenceLevel < 0)
        //    {
        //        figure.Segments.Add(new LineSegment(new Point(0, horizontalCenter + height - 2), true));
        //        figure.Segments.Add(new LineSegment(new Point(verticalCenter, horizontalCenter + height), true));
        //        figure.Segments.Add(new LineSegment(new Point(width, horizontalCenter + height - 2), true));
        //        figure.Segments.Add(new LineSegment(new Point(width, horizontalCenter), true));
        //    }
        //    else
        //    {
        //        figure.Segments.Add(new LineSegment(new Point(0, horizontalCenter - height + 2), true));
        //        figure.Segments.Add(new LineSegment(new Point(verticalCenter, horizontalCenter - height), true));
        //        figure.Segments.Add(new LineSegment(new Point(width, horizontalCenter - height + 2), true));
        //        figure.Segments.Add(new LineSegment(new Point(width, horizontalCenter), true));
        //    }

        //    var geometry = new PathGeometry(new[] { figure });

        //    var drawing = new GeometryDrawing(brush, new Pen(Brushes.White, 0), geometry);
        //    var image = new DrawingImage(drawing);

        //    return image;
        //}
    }
}

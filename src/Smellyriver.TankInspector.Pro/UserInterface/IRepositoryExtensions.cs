using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.UserInterface
{
    public static class IRepositoryExtensions
    {
        private class MarkerCacheInfo
        {
            public Color MarkerColor;
            public ImageSource Marker;
        }

        private static Dictionary<IRepository, MarkerCacheInfo> s_repositoryMarkers
             = new Dictionary<IRepository, MarkerCacheInfo>();

        public static ImageSource GetMarker(this IRepository repository)
        {
            MarkerCacheInfo cachedMarker;
            var markerColor = RepositoryManager.Instance.GetConfiguration(repository).MarkerColor;
            if(s_repositoryMarkers.TryGetValue(repository, out cachedMarker))
            {
                if (cachedMarker.MarkerColor == markerColor)
                    return cachedMarker.Marker;
            }

            cachedMarker = new MarkerCacheInfo();
            cachedMarker.MarkerColor = markerColor;
            cachedMarker.Marker = IRepositoryExtensions.CreateRepositoryMarker(repository);
            s_repositoryMarkers[repository] = cachedMarker;

            return cachedMarker.Marker;
        }

        private static ImageSource CreateRepositoryMarker(IRepository repository)
        {
            var markerColor = RepositoryManager.Instance.GetConfiguration(repository).MarkerColor;
            var brush = new SolidColorBrush(markerColor);

            var grid = new Grid();

            var border = new Border
            {
                Background = brush,
                BorderBrush = Brushes.White,
                BorderThickness = new Thickness(1.0),
                CornerRadius = new CornerRadius(3.0),
                //Margin=new Thickness(2.0)
            };

            grid.Children.Add(border);

            var versionText = new TextBlock
            {
                Text = repository.Version.ShortVersionString,
                FontSize = 8,
                Margin = new Thickness(2.0, 1.0, 2.0, 1.0),
                Foreground = markerColor.GetLuminance() > 0.5 ? Brushes.Black : Brushes.White
            };

            border.Child = versionText;

            grid.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            grid.Arrange(new Rect(grid.DesiredSize));



            var image = grid.RenderToBitmap();

            //var brush = new SolidColorBrush(RepositoryManager.Instance.GetConfiguration(repository).MarkerColor);
            //var drawing = new GeometryDrawing(brush, new Pen(Brushes.White, 1), new EllipseGeometry(new Point(5, 5), 5, 5));
            //var image = new DrawingImage(drawing);
            image.Freeze();
            return image;
        }

        public static string GetName(this IRepository repository)
        {
            var configuration = RepositoryManager.Instance.GetConfiguration(repository);
            return string.IsNullOrEmpty(configuration.Alias) ? repository.Version.ToString() : configuration.Alias;
        }
    }
}

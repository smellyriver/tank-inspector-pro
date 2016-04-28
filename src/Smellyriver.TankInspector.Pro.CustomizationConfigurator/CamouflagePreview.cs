using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.IO;
using Smellyriver.TankInspector.Pro.CustomizationConfigurator.Effects;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Graphics;
using Smellyriver.TankInspector.Pro.Repository;
using WpfColor = System.Windows.Media.Color;

namespace Smellyriver.TankInspector.Pro.CustomizationConfigurator
{
    static class CamouflagePreview
    {
        public static ImageSource GetCamouflagePreview(Camouflage camouflage, WpfColor baseColor, IRepository repository)
        {
            var client = repository as LocalGameClient;
            if (client == null)
                return null;

            var cacheFilename = string.Format("camouflage_{0}_{1}.png", camouflage.Id, camouflage.Key);

            return client.CacheManager.Load(cacheFilename,
                                            () => CamouflagePreview.CreateCamouflagePreview(client, camouflage, baseColor),
                                            CamouflagePreview.LoadCamouflagePreview,
                                            CamouflagePreview.SaveCamouflagePreview);

        }

        private static void SaveCamouflagePreview(Stream stream, BitmapSource preview)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(preview));
            encoder.Save(stream);
        }

        private static BitmapSource LoadCamouflagePreview(Stream stream)
        {
            var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            var imageSource = decoder.Frames.First();
            imageSource.Freeze();
            return imageSource;
        }

        private static BitmapSource CreateCamouflagePreview(LocalGameClient client, Camouflage camouflage, WpfColor baseColor)
        {
            if (!PackageStream.IsFileExisted(client.PackageIndexer, camouflage.Texture))
                return null;

            var image = new Image();
            image.Width = 64;
            image.Height = 64;
            
            using (var stream = new PackageStream(client.PackageIndexer, camouflage.Texture))
            {
                image.Source = (new DDSImage(stream)).BitmapSource(0);
            }

            var colors = camouflage.GetColors();

            image.Effect = new CamouflageEffect()
                {
                    BaseColor = baseColor,
                    Color1 = colors[0],
                    Color2 = colors[1],
                    Color3 = colors[2],
                    Color4 = colors[3],
                };

            image.MeasureAndArrange();
            var imageSource = image.RenderToBitmap();
            imageSource.Freeze();
            return imageSource;

        }
    }
}

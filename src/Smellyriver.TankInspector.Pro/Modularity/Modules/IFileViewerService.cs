using System.IO;
using System.Windows;
using Smellyriver.TankInspector.Pro.IO;
using Smellyriver.TankInspector.Pro.Modularity.Features;

namespace Smellyriver.TankInspector.Pro.Modularity.Modules
{
    public interface IFileViewerService
    {
        string Name { get; }
        FileTypeInfo[] SupportedFileTypes { get; }
        FrameworkElement CreateViewer(Stream fileStream, out IFeature[] features);
        FrameworkElement CreateViewer(string path, out IFeature[] features);
    }
}

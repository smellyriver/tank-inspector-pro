using Smellyriver.TankInspector.Pro.IO;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Features;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

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

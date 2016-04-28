using System.IO;
using System.Windows;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.Pro.IO;
using Smellyriver.TankInspector.Pro.Modularity.Features;
using Smellyriver.TankInspector.Pro.Modularity.Modules;

namespace Smellyriver.TankInspector.Pro.XmlViewerService
{
    public class XmlViewerService : IFileViewerService
    {
        public string Name
        {
            get { return this.L("xml_viewer_service", "service_name"); }
        }

        public FileTypeInfo[] SupportedFileTypes
        {
            get
            {
                var xmlFileIcon = BitmapImageEx.LoadAsFrozen("Resources/Images/XML_16.png");
                return new FileTypeInfo[]
                {
                    new FileTypeInfo("xml",         this.L("xml_viewer_service", "file_type_xml"),          xmlFileIcon),
                    new FileTypeInfo("def",         this.L("xml_viewer_service", "file_type_def"),          xmlFileIcon),
                    new FileTypeInfo("visual",      this.L("xml_viewer_service", "file_type_visual"),       xmlFileIcon),
                    new FileTypeInfo("settings",    this.L("xml_viewer_service", "file_type_settings"),     xmlFileIcon),
                    new FileTypeInfo("model",       this.L("xml_viewer_service", "file_type_model"),        xmlFileIcon),
                    new FileTypeInfo("animation",   this.L("xml_viewer_service", "file_type_animation"),    xmlFileIcon),
                    new FileTypeInfo("track",       this.L("xml_viewer_service", "file_type_track"),        xmlFileIcon),
                };
            }
        }

        public XmlViewerServiceModule Module { get; private set; }

        public XmlViewerService(XmlViewerServiceModule module)
        {
            this.Module = module;
        }

        public FrameworkElement CreateViewer(Stream fileStream, out IFeature[] features)
        {
            var viewer = new XmlViewer();
            var viewerVm = new XmlViewerVM(this, viewer.CommandBindings, fileStream);
            viewer.ViewModel = viewerVm;

            features = new IFeature[] { viewerVm };

            return viewer;
        }

        public FrameworkElement CreateViewer(string path, out IFeature[] features)
        {
            var viewer = new XmlViewer();
            var viewerVm = new XmlViewerVM(this, viewer.CommandBindings, path);
            viewer.ViewModel = viewerVm;

            features = new IFeature[] { viewerVm };

            return viewer;
        }

    }
}

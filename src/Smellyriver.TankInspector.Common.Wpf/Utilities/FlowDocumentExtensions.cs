using System.IO;
using System.IO.Packaging;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;

namespace Smellyriver.TankInspector.Common.Wpf.Utilities
{
    public static class FlowDocumentExtensions
    {
        public static void SaveAsXps(this FlowDocument document, string path)
        {
            using (var package = Package.Open(path, FileMode.Create))
            {
                using (var xpsDoc = new XpsDocument(package, CompressionOption.Maximum))
                {
                    var serializationManager = new XpsSerializationManager(new XpsPackagingPolicy(xpsDoc), false);
                    var paginator = ((IDocumentPaginatorSource)document).DocumentPaginator;
                    serializationManager.SaveAsXaml(paginator);
                }
            }
        }
    }
}

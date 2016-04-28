using System.IO;
using System.Windows.Documents;
using System.Windows.Markup;
using Smellyriver.TankInspector.Pro.Globalization;

namespace Smellyriver.TankInspector.Pro.UserInterface
{
    static class FlowDocumentHelper
    {
        public static FlowDocument LoadDocument(string filename)
        {
            var path = Localization.Instance.GetLocalizedFile(filename);

            if (path == null)
                return null;

            using (var file = File.OpenRead(path))
                return (FlowDocument)XamlReader.Load(file);
        }
    }
}

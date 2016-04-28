using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smellyriver.TankInspector.Pro.Modularity.Modules
{
    public static class IFileViewerServiceExtensions
    {
        public static string GetFilter(this IFileViewerService service, bool generateAllFiles = true)
        {
            var builder = new StringBuilder();
            foreach(var fileType in service.SupportedFileTypes)
            {
                builder.AppendFormat("{0} (*.{1})|*.{1}|", fileType.Description, fileType.ExtensionName);
            }

            if (generateAllFiles)
                builder.Append("All Files(*.*)|*.*");
            else
                builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }
    }
}

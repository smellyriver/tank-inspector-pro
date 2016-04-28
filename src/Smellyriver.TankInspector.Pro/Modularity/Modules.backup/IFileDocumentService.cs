using Smellyriver.TankInspector.Pro.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Smellyriver.TankInspector.Pro.Modularity.Modules
{
    public interface IFileDocumentService
    {
        ImageSource GetIconSource(string extensionName);
        bool HasFileViewerService(string extensionName);
        Uri CreateTemporaryStreamUri(string category, string extensionName, string path, Stream stream, string title, string description, IRepository ownerRepository);
        void RegisterViewer(IFileViewerService service);
    }
}

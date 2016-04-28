using Smellyriver.TankInspector.Pro.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Smellyriver.TankInspector.Pro.Modularity.Modules
{
    partial class FileDocumentService
    {
        private class StreamInfo
        {
            public string FileType;
            public WeakReference<Stream> StreamReference;
            public string Title;
            public string Description;
            public IRepository OwnerRepository;
            public Uri Uri;
        }
    }
}

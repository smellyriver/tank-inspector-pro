using System;
using System.IO;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Pro.Repository;

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

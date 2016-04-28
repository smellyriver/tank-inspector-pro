using System.IO;
using Smellyriver.TankInspector.IO;
using Smellyriver.TankInspector.Pro.IO;

namespace Smellyriver.TankInspector.Pro.Repository
{
    public sealed class LocalGameClientFileLocator : IFileLocator
    {
        public FileSource Source { get; }

        public LocalGameClient Client { get; }

        public LocalGameClientFileLocator(LocalGameClient client, FileSource source)
        {
            this.Client = client;
            this.Source = source;
        }

        public Stream OpenRead(string path)
        {
            Stream stream;
            if (this.TryOpenRead(path, out stream))
                return stream;

            return null;
        }


        public bool TryOpenRead(string path, out Stream stream)
        {
            if (this.Source == FileSource.ModFolder)
            {
                var modFile = Path.Combine(this.Client.ModDirectory, path);
                if (File.Exists(modFile))
                {
                    stream = File.OpenRead(modFile);
                    return true;
                }
            }

            if(PackageStream.IsFileExisted(this.Client.PackageIndexer, path))
            {
                stream = new PackageStream(this.Client.PackageIndexer, path);
                return true;
            }

            stream = null;
            return false;
        }
    }
}

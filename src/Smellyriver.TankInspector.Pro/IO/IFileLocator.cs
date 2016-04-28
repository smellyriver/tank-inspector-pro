using System.IO;

namespace Smellyriver.TankInspector.Pro.IO
{
    public interface IFileLocator
    {
        Stream OpenRead(string path);
        bool TryOpenRead(string path, out Stream stream);
    }
}

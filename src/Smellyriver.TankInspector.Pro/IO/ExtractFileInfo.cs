using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Smellyriver.TankInspector.Pro.IO
{
    class ExtractFileInfo
    {
        public string PackagePath { get; set; }
        public ZipEntry Entry { get; set; }
        public ZipFile ZipFile { get; set; }
        public string RelativePath { get; set; }
        public string DestinationRoot { get; set; }

        public string DestinationPath
        {
            get { return Path.Combine(this.DestinationRoot, this.RelativePath); }
        }
        public bool IsExisted { get; set; }
        public bool ShouldOverwrite { get; set; }
    }
}

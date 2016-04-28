using System.Windows.Media;

namespace Smellyriver.TankInspector.Pro.IO
{
    public struct FileTypeInfo
    {
        public string ExtensionName { get; set; }
        public string Description { get; set; }
        public ImageSource IconSource { get; set; }

        public FileTypeInfo(string extensionName, string description)
            : this()
        {
            this.ExtensionName = extensionName;
            this.Description = description;
        }

        public FileTypeInfo(string extensionName, string description, ImageSource iconSource)
            : this(extensionName, description)
        {
            this.IconSource = iconSource;
        }
    }
}

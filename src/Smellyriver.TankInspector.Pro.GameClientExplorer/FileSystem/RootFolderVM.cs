using System;
using System.Linq;
using System.Windows.Media;
using Smellyriver.TankInspector.Pro.Globalization;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Common.Utilities;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer.FileSystem
{
    class RootFolderVM : FolderVM
    {

        public override ImageSource IconSource
        {
            get { return NodeIconService.Current.GetNodeIcon(NodeTypes.RootFolder); }
        }

        public RootFolderVM(LocalGameClientNodeVM parent, LocalGameClient client)
            : base(parent, 
                   client.RootPath,
                   Localization.Instance.L("game_client_explorer", "files_folder"))
        {

        }

        internal void ExpandToFile(string path)
        {
            var segments = PathEx.Relativize(path, this.Path).Split(new[] { '/', '\\', '>' }, StringSplitOptions.RemoveEmptyEntries);
            FileSystemObjectVM node = this;
            for (var i = 0; i < segments.Length; ++i)
            {
                node.EnsureChildrenLoaded();
                node = node.Children.Cast<FileSystemObjectVM>()
                                    .FirstOrDefault(c => string.Equals(c.Name, segments[i], StringComparison.InvariantCultureIgnoreCase));
                if (node == null)
                    return;
            }

            if (node != null)
            {
                node.IsExpanded = true;
                node.IsSelected = true;
            }

        }
    }
}

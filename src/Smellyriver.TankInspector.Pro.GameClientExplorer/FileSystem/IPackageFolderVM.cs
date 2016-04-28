using System.Collections.ObjectModel;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.TreeView;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer.FileSystem
{
    interface IPackageFolderVM : IPackageFileSystemObjectVM
    {
        ObservableCollection<TreeNodeVM> Children { get; }
    }
}

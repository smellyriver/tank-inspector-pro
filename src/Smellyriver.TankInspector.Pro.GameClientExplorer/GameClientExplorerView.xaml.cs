using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Smellyriver.TankInspector.Pro.GameClientExplorer.FileSystem;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer
{
    public partial class GameClientExplorerView : UserControl
    {
        internal GameClientExplorerVM ViewModel
        {
            get { return this.DataContext as GameClientExplorerVM; }
            set { this.DataContext = value; }
        }

        public GameClientExplorerView()
        {
            InitializeComponent();
        }

        private TreeViewItem FileTreeHitTest(MouseButtonEventArgs e)
        {
            var element = FileTree.InputHitTest(e.GetPosition(FileTree)) as DependencyObject;
            while (!(element is TreeViewItem) && element != null)
                element = VisualTreeHelper.GetParent(element);

            var item = (TreeViewItem)element;

            if (item != null)
                item.IsSelected = true;

            return item;
        }

        private void FileTree_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = this.FileTreeHitTest(e);
            if (item == null)
                return;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {

                var viewModel = item.DataContext as FileSystemObjectVM;
                if (viewModel == null)
                    return;

                if (item.DataContext is IPackageFileSystemObjectVM)
                    return;

                ShellContextMenu.ShowFolderContextMenu(viewModel.Path);
                e.Handled = true;

                this.ViewModel.IsControlKeyPressed = false;

            }
        }

        private void FileTreeItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = this.FileTreeHitTest(e);
            if (item == null)
                return;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                var viewModel = item.DataContext as FileSystemObjectVM;
                if (viewModel == null)
                    return;

                if (item.DataContext is IPackageFileSystemObjectVM)
                    return;

                ShellContextMenu.ShowFolderContextMenu(viewModel.Path);
                e.Handled = true;

                this.ViewModel.IsControlKeyPressed = false;
            }
        }

        private void FileTreeItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = sender as TreeViewItem;
            if (item == null)
                return;

            var viewModel = item.DataContext as ExplorerTreeNodeVM;
            if (viewModel == null)
                return;

            if (viewModel.Children.Count > 0)
                return;

            if (viewModel.DefaultCommand == null)
                return;

            viewModel.DefaultCommand.Execute(null);
            e.Handled = true;
        }


        private void UserControl_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                this.ViewModel.IsControlKeyPressed = false;
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                this.ViewModel.IsControlKeyPressed = true;
        }

        private void FileTree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {            //var item = this.FileTreeHitTest(e);
            //if (item != null)
            //{

            //    var viewModel = item.DataContext as ExplorerTreeNodeVM;
            //    if (viewModel == null)
            //        return;

            //    if (viewModel.Children.Count > 0)
            //        return;

            //    if (viewModel.DefaultCommand == null)
            //        return;

            //    viewModel.DefaultCommand.Execute(null);
            //    e.Handled = true;
            //}

        }



    }
}

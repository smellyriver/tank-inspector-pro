using System.Windows.Input;
using MahApps.Metro.Controls;
using Smellyriver.TankInspector.Pro.UserInterface.ViewModels;

namespace Smellyriver.TankInspector.Pro.UserInterface
{
    public partial class Shell : MetroWindow
    {
        internal ShellVM ViewModel
        {
            get { return this.DataContext as ShellVM; }
            set { this.DataContext = value; }
        }

        public Shell()
        {
            this.ViewModel = new ShellVM(this.CommandBindings);
            InitializeComponent();
        }

        private void ShowAboutDialog_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (new About()).ShowDialog();
        }
    }
}

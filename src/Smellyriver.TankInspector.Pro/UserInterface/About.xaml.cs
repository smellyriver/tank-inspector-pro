using System;
using System.Windows;
using System.Windows.Documents;
using MahApps.Metro.Controls;
using Smellyriver.TankInspector.Pro.UserInterface.ViewModels;

namespace Smellyriver.TankInspector.Pro.UserInterface
{
    public partial class About : MetroWindow
    {
        public About()
        {
            InitializeComponent();

            this.DataContext = new AboutVM();
            this.CreditsViewer.Document = FlowDocumentHelper.LoadDocument("credits.xaml");
            this.LegalNotesViewer.Document = Application.LoadComponent(new Uri("Resources/Documents/LegalNotices.xaml", UriKind.Relative)) as FlowDocument;
        }
    }

}

using System;
using System.Windows;
using System.Windows.Input;
using Smellyriver.TankInspector.Pro.InternalModules.RepositoryManager;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Menus;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.Menu;

namespace Smellyriver.TankInspector.Pro.InternalModules
{
    class InternalDocumentService : IDocumentService
    {
        public static InternalDocumentService Instance { get; private set; }
        static InternalDocumentService()
        {
            InternalDocumentService.Instance = new InternalDocumentService();
        }

        public const string InternalDocumentScheme = "stipro";
        public const string RepositoryManagerDomain = "repositories";

        public static readonly Uri RepositoryManagerUri = new Uri(string.Format("{0}://{1}", InternalDocumentScheme, RepositoryManagerDomain));

        private readonly Guid _guid = Guid.Parse("3178531A-4ADD-46C5-B227-73D4EFE6037D");
        public Guid Guid
        {
            get { return _guid; }
        }

        public string[] SupportedSchemes
        {
            get { return new[] { InternalDocumentScheme }; }
        }


        private InternalDocumentService()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(InternalModuleCommands.ManageGameClients, ExecuteManageGameClients));
            MenuManager.Instance.Register(new MenuItemVM(this.L("game_client_manager", "manage_game_clients_menu_item"), 
                                                         InternalModuleCommands.ManageGameClients)
            {
                
            }, MenuAnchor.Tools);
        }

        private void ExecuteManageGameClients(object sender, ExecutedRoutedEventArgs e)
        {
            DockingViewManager.Instance.DocumentManager.OpenDocument(RepositoryManagerUri);
        }
        

        public ICreateDocumentTask CreateCreateDocumentTask(Uri uri, Guid guid, string persistentInfo)
        {
            return CreateDocumentTask.FromFactory(() =>
                {
                    var docType = uri.Host;

                    switch (docType)
                    {
                        case RepositoryManagerDomain:
                            return RepositoryManagerDocumentService.Instance.CreateDocument(uri, guid, persistentInfo);
                        default:
                            throw new NotSupportedException();
                    }
                });
        }
    }
}

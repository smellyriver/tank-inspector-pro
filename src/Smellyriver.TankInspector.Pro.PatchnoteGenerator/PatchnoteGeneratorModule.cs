using System;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Input;
using Smellyriver.TankInspector.Pro.Modularity.Modules;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Pro.UserInterface;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Menus;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups;
using Smellyriver.TankInspector.Common.Wpf.Input;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.Menu;
using Smellyriver.TankInspector.Common.Wpf.Utilities;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator
{
    [ModuleExport("PatchnoteGenerator", typeof(PatchnoteGeneratorModule))]
    [ExportMetadata("Guid", "8AA14654-1F85-4186-B362-1414EF4FEEAE")]
    [ExportMetadata("Name", "#patchnote_generator:module_name")]
    [ExportMetadata("Description", "#patchnote_generator:module_description")]
    [ExportMetadata("Version", "1.0.0.0")]
    [ExportMetadata("Provider", "#patchnote_generator:module_provider")]
    public class PatchnoteGeneratorModule : FlowDocumentModuleBase
    {
        private readonly static Guid s_createPatchnoteCommandGuid = Guid.Parse("BB151079-0F13-4B31-910C-473397450319");
        private readonly MenuItemVM _newPatchnoteMenuItem;

        public PatchnoteGeneratorModule()
        {
            _newPatchnoteMenuItem = new MenuItemVM(this.L("patchnote_generator", "new_patchnote_menu_item"))
            {
                Icon = BitmapImageEx.LoadAsFrozen("Resources/Images/Patchnote_16.png")
            };
        }

        public override void Initialize()
        {
            base.Initialize();

            DocumentServiceManager.Instance.Register(PatchnoteGeneratorDocumentService.Instance);

            ((INotifyCollectionChanged)RepositoryManager.Instance.Repositories).CollectionChanged += Repositories_CollectionChanged;

            MenuManager.Instance.Register(_newPatchnoteMenuItem, MenuAnchor.New);
            this.UpdateNewMenuItems();

            RepositoryCommandManager.Instance.Register(new RepositoryCommand(guid: s_createPatchnoteCommandGuid,
                                                                             name: this.L("patchnote_generator", "create_patchnote_menu_item"),
                                                                             execute: this.ExecuteCreatePatchnote,
                                                                             icon: BitmapImageEx.LoadAsFrozen("Resources/Images/Patchnote_16.png")));
        }

        private void ExecuteCreatePatchnote(string id)
        {
            if (RepositoryManager.Instance.Repositories.Count < 2)
            {
                DialogManager.Instance.ShowMessageAsync(this.L("patchnote_generator", "insufficient_game_clients_message_title"),
                                                        this.L("patchnote_generator", "insufficient_game_clients_message"));

                return;
            }

            var flyoutVm = new CreatePatchnoteFlyoutVM(id);
            var flyout = new Flyout
            {
                Header = this.L("patchnote_generator", "create_patchnote_flyout_title"),
                IsModal = true,
                Content = new CreatePatchnoteFlyoutView { ViewModel = flyoutVm },
                CloseCommand = new RelayCommand(() => this.OnCreatePatchnoteFlyoutClosed(flyoutVm)),
                Position = FlyoutPosition.Left,
                MinWidth = 200
            };
            FlyoutManager.Instance.Open(flyout);

            flyoutVm.Closed += (o, e) =>
                {
                    FlyoutManager.Instance.Close(flyout);
                    this.OnCreatePatchnoteFlyoutClosed(flyoutVm);
                };
        }


        private void OnCreatePatchnoteFlyoutClosed(CreatePatchnoteFlyoutVM flyoutVm)
        {
            if (flyoutVm.IsProceed)
            {
                this.CreatePatchnote(flyoutVm.SelectedRepositoryPair.Target, flyoutVm.SelectedRepositoryPair.Reference);
            }
        }

        void Repositories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateNewMenuItems();
        }



        private void UpdateNewMenuItems()
        {
            _newPatchnoteMenuItem.MenuItems.Clear();

            if (RepositoryManager.Instance.Repositories.Count < 2)
            {
                _newPatchnoteMenuItem.IsEnabled = false;
                return;
            }

            _newPatchnoteMenuItem.IsEnabled = true;

            foreach (var repository in RepositoryManager.Instance.Repositories)
            {
                var repositoryMenuVm = new MenuItemVM(RepositoryHelper.GetRepositoryDisplayName(repository))
                {
                    Icon = repository.GetMarker()
                };

                foreach (var repository2 in RepositoryManager.Instance.Repositories)
                {
                    if (repository == repository2)
                        continue;

                    var createPatchnoteMenuVm = new MenuItemVM(this.L("patchnote_generator", "new_patchnote_from_client_menu_item", RepositoryHelper.GetRepositoryDisplayName(repository2)),
                                                               new RelayCommand<IRepository[]>(this.CreatePatchnote),
                                                               new[] { repository2, repository })
                    {
                        Icon = repository2.GetMarker()
                    };

                    repositoryMenuVm.MenuItems.Add(createPatchnoteMenuVm);
                }

                foreach (var repository2 in RepositoryManager.Instance.Repositories)
                {
                    if (repository == repository2)
                        continue;

                    var createPatchnoteMenuVm = new MenuItemVM(this.L("patchnote_generator", "new_patchnote_to_client_menu_item", RepositoryHelper.GetRepositoryDisplayName(repository2)),
                                                               new RelayCommand<IRepository[]>(this.CreatePatchnote),
                                                               new[] { repository, repository2 })
                    {
                        Icon = repository2.GetMarker()
                    };

                    repositoryMenuVm.MenuItems.Add(createPatchnoteMenuVm);
                }

                _newPatchnoteMenuItem.MenuItems.Add(repositoryMenuVm);
            }

        }

        private void CreatePatchnote(IRepository[] repositories)
        {
            this.CreatePatchnote(repositories[1], repositories[0]);
        }

        private void CreatePatchnote(IRepository target, IRepository reference)
        {
            DockingViewManager.Instance.DocumentManager.OpenDocument(PatchnoteGeneratorDocumentService.CreateUri(target, reference));
        }

        private void DoSomething()
        {
            DockingViewManager.Instance.DocumentManager.OpenDocument(PatchnoteGeneratorDocumentService.CreateUri(RepositoryManager.Instance.Repositories[1], RepositoryManager.Instance.Repositories[0]));
        }

    }
}

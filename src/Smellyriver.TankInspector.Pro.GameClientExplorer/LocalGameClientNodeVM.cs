using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using Smellyriver.TankInspector.Pro.GameClientExplorer.FileSystem;
using Smellyriver.TankInspector.Pro.GameClientExplorer.Vehicles;
using Smellyriver.TankInspector.Pro.Modularity.Input;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Pro.UserInterface;
using Smellyriver.TankInspector.Common.Wpf.Input;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.TreeView;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using IWpfCommand = System.Windows.Input.ICommand;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer
{
    class LocalGameClientNodeVM : ExplorerTreeNodeVM
    {

        public override string Description
        {
            get { return this.Model.Description; }
        }

        public override ImageSource IconSource
        {
            get { return this.Model.GetMarker(); }
        }

        public override LocalGameClientNodeVM GameClientRoot
        {
            get { return this; }
        }

        public LocalGameClient Model { get; }

        public GameClientExplorerVM FileExplorer { get; private set; }

        public VehiclesFolderVM VehiclesNode { get; }
        public DataFolderVM DataNode { get; }
        public RootFolderVM FilesNode { get; }

        private IWpfCommand ShowPropertiesCommand;

        public RepositoryConfiguration Configuration { get; }

        public LocalGameClientNodeVM(GameClientExplorerVM owner, LocalGameClient client)
            : base(null, client.Name, LoadChildenStrategy.Manual)
        {
            this.Configuration = RepositoryManager.Instance.GetConfiguration(client);
            this.Name = this.Configuration.Alias;
            this.Configuration.PropertyChanged += Configuration_PropertyChanged;

            this.FileExplorer = owner;
            this.Model = client;

            this.VehiclesNode = new VehiclesFolderVM(this, this.Model);
            this.InternalChildren.Add(this.VehiclesNode);

            this.DataNode = new DataFolderVM(this, this.Model);
            this.InternalChildren.Add(this.DataNode);

            this.FilesNode = new RootFolderVM(this, this.Model);
            this.InternalChildren.Add(this.FilesNode);

            this.ShowPropertiesCommand = new RelayCommand(this.ShowProperties);
        }

        private void Configuration_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Alias":
                    this.Name = this.Configuration.Alias;
                    break;

                case "MarkerColor":
                    RaisePropertyChanged(() => this.IconSource);
                    break;
            }
        }

        private void ShowProperties()
        {
            RepositoryManager.Instance.OpenRepositoryPropertiesDocument(this.Model);
        }

        protected override List<ExplorerTreeContextMenuItemVM> CreateContextMenuItems()
        {
            var list = base.CreateContextMenuItems();

            foreach (var command in RepositoryCommandManager.Instance.Commands)
            {
                list.Add(new ExplorerTreeContextMenuItemVM(command.Priority,
                                                           command.Name,
                                                           command,
                                                           this.Model.ID,
                                                           command.Icon));
            }

            list.Add(new ExplorerTreeContextMenuItemVM(order: 0,
                                                       name: this.L("game_client_explorer", "properties_menu_item"),
                                                       command: this.ShowPropertiesCommand,
                                                       iconSource: BitmapImageEx.LoadAsFrozen("Resources/Images/Actions/ShowProperties_16.png")));
            return list;
        }
    }
}

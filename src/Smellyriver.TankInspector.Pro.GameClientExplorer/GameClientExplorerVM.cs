using System.Collections.Specialized;
using System.Windows.Input;
using System.Windows.Threading;
using Smellyriver.Collections;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Common.Collections;
using Smellyriver.TankInspector.Common.Wpf.Behaviors.DragDrop;
using Smellyriver.TankInspector.Common.Wpf.Input;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.GameClientExplorer
{
    class GameClientExplorerVM : NotificationObject
    {
        private readonly ViewModelMap<IRepository, LocalGameClientNodeVM> _gameClients;
        public INotifyCollectionChanged GameClients { get { return _gameClients; } }

        private bool _isControlKeyPressed;

        public bool IsControlKeyPressed
        {
            get { return _isControlKeyPressed; }
            set
            {
                _isControlKeyPressed = value;
                this.RaisePropertyChanged(() => this.IsControlKeyPressed);
            }
        }


        public GameClientExplorerModule Module { get; private set; }

        public Dispatcher Dispatcher { get; private set; }

        public IDragSource ExplorerTreeDragHandler { get; private set; }

        public ICommand ManageRepositoriesCommand { get; private set; }
             
        public GameClientExplorerVM(GameClientExplorerModule module, Dispatcher dispatcher)
        {
            this.Module = module;
            this.Dispatcher = dispatcher;

            _gameClients = new ViewModelMap<IRepository, LocalGameClientNodeVM>(RepositoryManager.Instance.Repositories,
                r => new LocalGameClientNodeVM(this, ((LocalGameClient)r)),
                null,
                r => r is LocalGameClient);

            this.ExplorerTreeDragHandler = new ExplorerTreeDragHandlerImpl();
            this.ManageRepositoriesCommand = new RelayCommand(this.ManageRepositories);
        }

        private void ManageRepositories()
        {
            RepositoryManager.Instance.OpenRepositoryManagerDocument();
        }

    }
}

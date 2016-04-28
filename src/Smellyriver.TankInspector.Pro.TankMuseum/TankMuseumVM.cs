using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Xml.XPath;
using Smellyriver.Collections;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Common.Collections;
using Smellyriver.TankInspector.Common.Wpf.Behaviors.DragDrop;
using Smellyriver.TankInspector.Common.Wpf.Input;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.Menu;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Modularity.Input;
using Smellyriver.TankInspector.Pro.Repository;
using IWpfCommand = System.Windows.Input.ICommand;

namespace Smellyriver.TankInspector.Pro.TankMuseum
{
    partial class TankMuseumVM : NotificationObject, IDataErrorInfo 
    {

        private readonly ViewModelMap<IRepository, TankVM[]> _allTanks;

        private ICollectionView _listedTanks;
        public ICollectionView ListedTanks
        {
            get { return _listedTanks; }
            set
            {
                _listedTanks = value;
                this.RaisePropertyChanged(() => this.ListedTanks);
            }
        }

        public IDragSource TankListDragHandler { get; private set; }

        public ObservableCollection<MenuItemVM> ContextMenuItems { get; }

        private TankVM[] _selectedTanks;

        public TankVM[] SelectedTanks
        {
            get { return _selectedTanks; }
            set
            {
                _selectedTanks = value;
                this.UpdateContextMenu();
            }
        }

        public IWpfCommand ResetFiltersCommand { get; private set; }

        public TankMuseumVM()
        {
            _allTanks = new ViewModelMap<IRepository, TankVM[]>(RepositoryManager.Instance.Repositories,
                                                                r =>
                                                                {
                                                                    var repositoryVm = new RepositoryVM(r);
                                                                    return r.TankDatabase.QueryMany("tank")
                                                                                         .Select(t => new TankVM(repositoryVm, new Tank(t))).ToArray();
                                                                });

            _allTanks.CollectionChanged += _allTanks_CollectionChanged;

            this.ListedTanks = CollectionViewSource.GetDefaultView(_allTanks);
            this.ListedTanks.Filter = this.Filter;

            this.TankListDragHandler = new TankListDragHandlerImpl();
            this.ContextMenuItems = new ObservableCollection<MenuItemVM>();
            this.ContextMenuItems.Add(new MenuItemVM("text"));

            this.ResetFiltersCommand = new RelayCommand(this.ResetFilters);
        }

        void _allTanks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            this.ListedTanks = CollectionViewSource.GetDefaultView(_allTanks.SelectMany(t => t));
            this.ListedTanks.Filter = this.Filter;
            this.UpdateFilters();
            this.FilterTanks();
        }


        private void UpdateContextMenu()
        {
            this.ContextMenuItems.Clear();

            if (this.SelectedTanks.Length == 0)
                return;

            var source = this.SelectedTanks.Length == 1
                       ? (IEnumerable<ICommand>)TankCommandManager.Instance.Commands.OrderBy(c => c.Priority)
                       : (IEnumerable<ICommand>)BulkTankCommandManager.Instance.Commands.OrderBy(c => c.Priority);

            var parameter = this.SelectedTanks.Length == 1
                       ? (object)this.SelectedTanks[0].TankUnikey
                       : (object)this.SelectedTanks.Select(t => t.TankUnikey).ToArray();

            foreach (var command in source)
            {
                this.ContextMenuItems.Add(new MenuItemVM(command.Name,
                                                         command,
                                                         parameter)
                {
                    Icon = command.Icon,
                });
            }

        }


        string IDataErrorInfo.Error
        {
            get { return string.Empty; }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                if (columnName == "FilterText")
                {
                    if(!string.IsNullOrEmpty(this.FilterText) && this.SelectedFilterMode.Model == FilterMode.XPath)
                    {
                        try
                        {
                            XPathExpression.Compile(this.FilterText);
                        }
                        catch(XPathException ex)
                        {
                            return ex.Message;
                        }
                    }
                }

                return null;
            }
        }
    }
}

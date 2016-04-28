using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.Common.Wpf.Behaviors.DragDrop;
using Smellyriver.TankInspector.Common.Wpf.Input;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    partial class StatComparisonDocumentVM
    {
        public TanksManagerVM TanksManager { get; }

        public class TanksManagerVM : NotificationObject
        {

            private readonly Dictionary<TankUnikey, TankVM> _tankVms;

            private readonly StatComparisonDocumentVM _owner;

            private TankVM _benchmarkTank;
            public TankVM BenchmarkTank
            {
                get { return _benchmarkTank; }
                set
                {
                    if (_benchmarkTank == value)
                        return;

                    if (_benchmarkTank != null)
                        _benchmarkTank.IsBenchmark = false;

                    _benchmarkTank = value;

                    if (_benchmarkTank != null)
                    {
                        foreach (var tank in this.SelectedTanks)
                            tank.IsBenchmark = tank == _benchmarkTank;
                    }

                    _owner.PersistentInfo.BenchmarkTankKey = _benchmarkTank == null ? (TankUnikey?)null : _benchmarkTank.TankUnikey;

                    if (!_owner._isInitializing)
                        _owner.UpdateBenchmarkings();
                }
            }

            private TankVM _selectedTank;
            public TankVM SelectedTank
            {
                get { return _selectedTank; }
                set
                {
                    _selectedTank = value;
                    this.RaisePropertyChanged(() => this.SelectedTank);

                    if (_selectedTank != null)
                    {
                        var tankInstance = TankInstanceManager.GetInstance(_selectedTank.Repository, _selectedTank.Model);
                        _owner.TankConfiguration = tankInstance.TankConfiguration;
                        _owner.CrewConfiguration = tankInstance.CrewConfiguration;
                    }
                    else
                    {
                        _owner.TankConfiguration = null;
                        _owner.CrewConfiguration = null;
                    }
                }
            }

            public IRepository SelectedTankRepository
            {
                get { return this.SelectedTank != null ? this.SelectedTank.Repository : null; }
            }


            public ObservableCollection<TankVM> SelectedTanks { get; }
            public ICommand RemoveTankCommand { get; private set; }
            public ICommand TankMoveUpCommand { get; private set; }
            public ICommand TankMoveDownCommand { get; private set; }
            public ICommand ToggleBenchmarkCommand { get; private set; }

            public ICommand LoadEliteConfigurationsCommand { get; private set; }
            public ICommand LoadFullProficiencyCrewsCommand { get; private set; }
            public ICommand LoadBrotherInArmsForAllCrewsCommand { get; private set; }
            public ICommand LoadCamouflageForAllCrewsCommand { get; private set; }

            public IDropTarget SelectedTanksDropHandler { get; private set; }

            public TanksManagerVM(StatComparisonDocumentVM owner)
            {
                _owner = owner;
                _tankVms = new Dictionary<TankUnikey, TankVM>();
                this.SelectedTanks = new ObservableCollection<TankVM>();

                foreach (var tankKey in _owner.PersistentInfo.TankKeys)
                {
                    var tank = this.GetTankVM(tankKey);
                    if (tank != null)
                        this.SelectedTanks.Add(tank);
                    else
                        this.LogWarning("tank '{0}' not found", tankKey);
                }

                if (_owner.PersistentInfo.BenchmarkTankKey != null)
                {
                    var benchmarkTank = this.GetTankVM(_owner.PersistentInfo.BenchmarkTankKey.Value);
                    if (benchmarkTank == null)
                        this.LogWarning("tank '{0}' not found", _owner.PersistentInfo.BenchmarkTankKey.Value);

                    if (!this.SelectedTanks.Contains(benchmarkTank))
                    {
                        this.LogWarning("invalid benchmark tank '{0}'", _owner.PersistentInfo.BenchmarkTankKey.Value);
                        _owner.PersistentInfo.BenchmarkTankKey = null;
                    }
                    else
                        this.BenchmarkTank = benchmarkTank;
                }

                this.SelectedTanks.CollectionChanged += SelectedTanks_CollectionChanged;

                this.InitializeCommands();

                this.SelectedTanksDropHandler = new SelectedTanksDropHandlerImpl(this);
            }

            private void InitializeCommands()
            {
                this.RemoveTankCommand = new RelayCommand<IList>(this.RemoveTank, this.CanDoBulkOperation);
                this.TankMoveUpCommand = new RelayCommand<IList>(this.TankMoveUp, this.CanMoveTankUp);
                this.TankMoveDownCommand = new RelayCommand<IList>(this.TankMoveDown, this.CanMoveTankDown);
                this.ToggleBenchmarkCommand = new RelayCommand<IList>(this.ToggleBenchmark, this.CanToggleBenchmark);

                this.LoadEliteConfigurationsCommand = new RelayCommand(this.LoadEliteConfigurations, this.CanDoBulkOperationToAllSelectedTanks);
                this.LoadFullProficiencyCrewsCommand = new RelayCommand(this.LoadFullProficiencyCrews, this.CanDoBulkOperationToAllSelectedTanks);
                this.LoadBrotherInArmsForAllCrewsCommand = new RelayCommand(this.LoadBrotherInArmsForAllCrews, this.CanDoBulkOperationToAllSelectedTanks);
                this.LoadCamouflageForAllCrewsCommand = new RelayCommand(this.LoadCamouflageForAllCrews, this.CanDoBulkOperationToAllSelectedTanks);
            }

            private void LoadCamouflageForAllCrews()
            {
                foreach (var tank in this.SelectedTanks)
                {
                    var tankInstance = TankInstanceManager.GetInstance(tank.Repository, tank.Model);
                    foreach (var crew in tankInstance.CrewConfiguration.Crews)
                        crew.LearnSkill("camouflage");
                }
            }

            private void LoadBrotherInArmsForAllCrews()
            {
                foreach (var tank in this.SelectedTanks)
                {
                    var tankInstance = TankInstanceManager.GetInstance(tank.Repository, tank.Model);
                    foreach (var crew in tankInstance.CrewConfiguration.Crews)
                        crew.LearnSkill("brotherhood");
                }
            }

            private void LoadFullProficiencyCrews()
            {
                foreach (var tank in this.SelectedTanks)
                {
                    var tankInstance = TankInstanceManager.GetInstance(tank.Repository, tank.Model);
                    foreach (var crew in tankInstance.CrewConfiguration.Crews)
                        crew.LastSkillTrainingLevel = 100;
                }
            }

            private void LoadEliteConfigurations()
            {
                foreach (var tank in this.SelectedTanks)
                {
                    TankInstanceManager.GetInstance(tank.Repository, tank.Model).TankConfiguration.LoadEliteConfiguration();
                }
            }


            internal TankVM GetTankVM(TankUnikey key)
            {
                TankVM tankVm;
                if (_tankVms.TryGetValue(key, out tankVm))
                    return tankVm;

                IRepository repository;
                IXQueryable tank;
                if (key.TryGetTank(out tank, out repository))
                {
                    tankVm = new TankVM(repository, tank);
                    _tankVms[key] = tankVm;
                    return tankVm;
                }

                return null;
            }

            internal TankVM GetTankVM(IRepository repository, IXQueryable tank)
            {
                var key = new TankUnikey(repository, tank);
                return _tankVms.GetOrCreate(key, () => new TankVM(repository, tank));
            }

            void SelectedTanks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                _owner.PersistentInfo.TankKeys = this.SelectedTanks.Select(t => new TankUnikey(t.Repository, t.Model)).ToArray();

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        _owner.AddRowOrColumns(e.NewItems.Cast<TankVM>(), e.NewStartingIndex);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        _owner.RemoveRowOrColumns(e.OldItems.Cast<TankVM>());
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        _owner.ReplaceRowOrColumns(e.OldItems.Cast<TankVM>(), e.NewItems.Cast<TankVM>());
                        break;
                    case NotifyCollectionChangedAction.Move:
                        _owner.MoveRowOrColumns(e.OldItems.Cast<TankVM>(), e.OldStartingIndex, e.NewStartingIndex);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        _owner.ResetTanks();
                        break;
                }
            }


            private bool CanToggleBenchmark(IList selectedItems)
            {
                return selectedItems.Count == 1;
            }

            private void ToggleBenchmark(IList selectedItems)
            {
                var currentTank = (TankVM)selectedItems[0];
                if (currentTank.IsBenchmark)
                    this.BenchmarkTank = null;
                else
                    this.BenchmarkTank = currentTank;
            }

            internal bool CanMoveTankDown(IList selectedItems)
            {
                return selectedItems.Count == 1
                    && this.SelectedTanks.Last() != selectedItems[0];
            }

            internal bool CanMoveTankUp(IList selectedItems)
            {
                return selectedItems.Count == 1
                    && this.SelectedTanks.First() != selectedItems[0];
            }

            internal bool CanDoBulkOperation(IList selectedItems)
            {
                return selectedItems != null && selectedItems.Count > 0;
            }

            internal bool CanDoBulkOperationToAllSelectedTanks()
            {
                return this.SelectedTanks.Count > 0;
            }

            internal void TankMoveDown(IList selectedItems)
            {
                var index = this.SelectedTanks.IndexOf((TankVM)selectedItems[0]);
                this.SelectedTanks.Move(index, index + 1);
            }

            internal void TankMoveUp(IList selectedItems)
            {
                var index = this.SelectedTanks.IndexOf((TankVM)selectedItems[0]);
                this.SelectedTanks.Move(index, index - 1);
            }

            internal void RemoveTank(IList selectedItems)
            {
                var tanks = selectedItems.Cast<TankVM>().ToArray();

                //var minIndex = int.MaxValue;
                foreach (var tank in tanks)
                {
                    if (tank == this.BenchmarkTank)
                        this.BenchmarkTank = null;

                    //minIndex = Math.Min(minIndex, this.SelectedTanks.IndexOf(tank));
                    this.SelectedTanks.Remove(tank);
                }

                this.SelectedTank = this.SelectedTanks.FirstOrDefault();
            }

            private void AddRangeIfNotExisted(IEnumerable<TankVM> tanks)
            {
                foreach (var tank in tanks)
                {
                    if (!this.SelectedTanks.Contains(tank))
                        this.SelectedTanks.Add(tank);
                }
            }

            private void AddIf(IRepository repository, Func<Tank, bool> predicate)
            {
                this.AddRangeIfNotExisted(repository.TankDatabase.QueryMany("tank")
                                                                 .Select(t => new Tank(t))
                                                                 .Where(predicate)
                                                                 .Select(t => this.GetTankVM(repository, t)));
            }


            internal void AddSameTierAndClassTanks(IRepository repository, int tier, string classKey)
            {
                this.AddIf(repository,
                           t => t.Tier == tier
                             && t.ClassKey == classKey);
            }

            internal void AddSameTierTanks(IRepository repository, int tier)
            {
                this.AddIf(repository,
                           t => t.Tier == tier);
            }

            internal void AddTankOfAllVersions(string key)
            {
                foreach (var repository in RepositoryManager.Instance.Repositories)
                    this.AddIf(repository,
                               t => t.Key == key);
            }
        }
    }
}

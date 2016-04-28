using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Smellyriver.TankInspector.Pro.Data.Stats;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Common.Utilities;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    partial class StatComparisonDocumentVM
    {
        public StatComparisonDocumentPersistentInfo PersistentInfo { get; }

        private ObservableCollection<IGridColumn> _columnHeaders;
        public ReadOnlyObservableCollection<IGridColumn> ColumnHeaders { get; private set; }

        private ObservableCollection<Dictionary<string, object>> _dataRows;
        public ReadOnlyObservableCollection<Dictionary<string, object>> DataRows { get; private set; }

        private Dictionary<StatInfoVM, Dictionary<TankVM, StatVM>> _statVms;

        private Func<IGridColumn, IGridColumn, StatVM> _StatVMGetter;
        private IEnumerable<IGridColumn> _columnCollection;
        private IEnumerable<IGridColumn> _rowCollection;

        private void InitializeGrid()
        {
            _columnHeaders = new ObservableCollection<IGridColumn>();
            this.ColumnHeaders = new ReadOnlyObservableCollection<IGridColumn>(_columnHeaders);

            _dataRows = new ObservableCollection<Dictionary<string, object>>();
            this.DataRows = new ReadOnlyObservableCollection<Dictionary<string, object>>(_dataRows);

            _statVms = new Dictionary<StatInfoVM, Dictionary<TankVM, StatVM>>();

            this.UpdateColumnModeVariables();
            this.ResetGrid();

            _dataRows.CollectionChanged += GridRowsOrColumns_CollectionChanged;
            _columnHeaders.CollectionChanged += GridRowsOrColumns_CollectionChanged;
        }

        void GridRowsOrColumns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RaisePropertyChanged(() => IsAddStatsAndTanksTipShown);
            this.UpdateAxisBuildingTasks();
        }

        private void UpdateAxisBuildingTasks()
        {
            var tasks = new List<AxisBuildingTask>();
            var sameTier = this.TanksManager.SelectedTanks.Equals(t => t.Model.Tier);
            if (this.TanksManager.SelectedTanks.Equals(t => t.Repository))
            {
                var repository = this.TanksManager.SelectedTanks[0].Repository;
                if (this.TanksManager.SelectedTanks.Equals(t => t.Model.Tier))
                {
                    var tier = this.TanksManager.SelectedTanks[0].Model.Tier;
                    if (this.TanksManager.SelectedTanks.Equals(t => t.Model.ClassKey))
                    {
                        tasks.Add(new AxisBuildingTask(this.L("stat_comparer", 
                                                              "add_same_tier_and_class_tanks_shortcut", 
                                                              tier,
                                                              this.TanksManager.SelectedTanks[0].Model.Class),
                                                       () => this.TanksManager.AddSameTierAndClassTanks(repository, 
                                                                                                        tier,
                                                                                                        this.TanksManager.SelectedTanks[0].Model.ClassKey)));
                    }

                    tasks.Add(new AxisBuildingTask(this.L("stat_comparer", "add_same_tier_tanks_shortcut", tier),
                                                   () => this.TanksManager.AddSameTierTanks(repository, tier)));
                }
            }

            if (RepositoryManager.Instance.Repositories.Count > this.TanksManager.SelectedTanks.Count)
            {
                if (this.TanksManager.SelectedTanks.Equals(t => t.Model.Key))
                {
                    var key = this.TanksManager.SelectedTanks[0].Model.Key;
                    var name = this.TanksManager.SelectedTanks[0].Name;
                    tasks.Add(new AxisBuildingTask(this.L("stat_comparer", "add_tank_of_all_versions_shortcut", name),
                                                   () => this.TanksManager.AddTankOfAllVersions(key)));
                }
            }

            if (this.StatsManager.SelectedStats.Count == 0)
            {
                if (File.Exists(StatsManagerVM.DefaultPresetFilePath))
                {
                    tasks.Add(new AxisBuildingTask(this.L("stat_comparer", "load_default_stats_preset_shortcut"),
                                                   () => this.StatsManager.LoadStatsPresetFile(StatsManagerVM.DefaultPresetFilePath)));
                }
            }

            this.AxisBuildingTasks = tasks;
        }

        private void ResetGrid()
        {
            this.ResetColumnHeaders();
            this.ResetValueRows();
        }

        private void UpdateColumnModeVariables()
        {
            if (this.ColumnMode.Mode == StatComparer.ColumnMode.Stats)
            {
                _rowCollection = this.TanksManager.SelectedTanks;
                _columnCollection = this.StatsManager.SelectedStats;
                _StatVMGetter = (r, c) => this.GetStatVM((TankVM)r, (StatInfoVM)c);
            }
            else
            {
                _rowCollection = this.StatsManager.SelectedStats;
                _columnCollection = this.TanksManager.SelectedTanks;
                _StatVMGetter = (r, c) => this.GetStatVM((TankVM)c, (StatInfoVM)r);
            }
        }


        private StatVM GetStatVM(TankVM tank, StatInfoVM statInfo)
        {
            var tankStats = _statVms.GetOrCreate(statInfo, () => new Dictionary<TankVM, StatVM>());
            return tankStats.GetOrCreate(tank,
                                         () => new StatVM(statInfo,
                                                          statInfo.Model,
                                                          TankInstanceManager.GetInstance(tank.Repository, tank.Model)));
        }

        private void ResetColumnHeaders()
        {
            _columnHeaders.Clear();
            _columnHeaders.AddRange(_columnCollection);
        }

        private void ResetValueRows()
        {
            _dataRows.Clear();
            this.AddRows(_rowCollection);
        }

        private void AddRows(IEnumerable<IGridColumn> rows, int index = -1)
        {
            if (index == -1)
                index = _dataRows.Count;

            foreach (var row in rows)
            {
                var dataRow = new Dictionary<string, object>();

                dataRow[RowHeaderKey] = row;

                foreach (var column in _columnCollection)
                    dataRow[column.Key] = _StatVMGetter(row, column);

                _dataRows.Insert(index, dataRow);
                ++index;
            }

            this.UpdateComparisonsAndBenchmarkingsOnAxisAdded(rows);
        }

        private void UpdateComparisonsAndBenchmarkingsOnAxisAdded(IEnumerable<IGridColumn> addedAxis)
        {
            var firstItem = addedAxis.FirstOrDefault();
            if (firstItem == null)
                return;

            if (firstItem is StatInfoVM)
            {
                foreach (var statInfo in addedAxis.Cast<StatInfoVM>())
                {
                    statInfo.StatValueLoaded += this.StatGroup_StatValueLoaded;
                    this.UpdateComparison(statInfo);
                    this.UpdateBenchmarking(statInfo);
                }
            }
            else
            {
                this.UpdateComparisons();
                this.UpdateBenchmarkings();
            }
        }

        private void UpdateComparisonsAndBenchmarkingsOnAxisRemoved(IEnumerable<IGridColumn> removedAxis)
        {
            var firstItem = removedAxis.FirstOrDefault();
            if (firstItem == null)
                return;

            if (removedAxis.First() is StatInfoVM)
            {
                foreach (var statInfo in removedAxis.Cast<StatInfoVM>())
                {
                    _statVms.Remove(statInfo);
                    statInfo.StatValueLoaded -= StatGroup_StatValueLoaded;
                }
            }
            else
            {
                foreach (var tankVm in removedAxis.Cast<TankVM>())
                {
                    foreach (var tankStats in _statVms.Values)
                    {
                        tankStats.Remove(tankVm);
                    }
                }

                this.UpdateComparisons();
                this.UpdateBenchmarkings();
            }
        }

        private void UpdateComparisons()
        {
            foreach (var statInfo in this.StatsManager.SelectedStats)
            {
                this.UpdateComparison(statInfo);
            }
        }

        private void UpdateComparison(StatInfoVM statInfo)
        {
            Dictionary<TankVM, StatVM> tankStats;
            if (!_statVms.TryGetValue(statInfo, out tankStats))
                return;

            if (statInfo.Model.CompareStrategy != CompareStrategy.HigherBetter
                && statInfo.Model.CompareStrategy != CompareStrategy.LowerBetter)
            {
                foreach (var stat in tankStats.Values)
                    stat.IsBest = false;

                return;
            }

            if (tankStats.Values.All(v => v.IsLoaded))
            {
                StatVM[] bestStats;

                // besure to convert the values to Array to make them threadsafe
                var statValues = tankStats.Values.ToArray();

                if (statInfo.Model.CompareStrategy == CompareStrategy.HigherBetter)
                    bestStats = statValues.WithMax(s => s.Value, statInfo.Model.Comparer);
                else
                    bestStats = statValues.WithMin(s => s.Value, statInfo.Model.Comparer);

                if (bestStats.Length == tankStats.Count)
                {
                    foreach (var stat in statValues)
                        stat.IsBest = false;
                }
                else
                {
                    var bestStatsSet = new HashSet<StatVM>(bestStats);

                    foreach (var stat in statValues)
                        stat.IsBest = bestStatsSet.Contains(stat);
                }
            }
        }

        public void UpdateBenchmarkings()
        {
            foreach (var statInfo in this.StatsManager.SelectedStats)
            {
                this.UpdateBenchmarking(statInfo);
            }
        }

        private void UpdateBenchmarking(StatInfoVM statInfo)
        {
            Dictionary<TankVM, StatVM> tankStats;
            if (!_statVms.TryGetValue(statInfo, out tankStats))
                return;

            var statVms = tankStats.Values.ToArray();

            if (this.TanksManager.BenchmarkTank == null)
            {
                foreach (var statVm in statVms)
                    statVm.BenchmarkIcon = null;

                return;
            }

            if (statVms.All(v => v.IsLoaded))
            {
                var benchmarkTankStatVm = tankStats[this.TanksManager.BenchmarkTank];
                benchmarkTankStatVm.BenchmarkIcon = null;

                foreach (var statVm in statVms)
                {
                    if (statVm != benchmarkTankStatVm)
                    {
                        statVm.BenchmarkIcon = statVm.GetBenchmarkIcon(benchmarkTankStatVm);
                    }
                }
            }
        }

        void StatGroup_StatValueLoaded(object sender, EventArgs e)
        {
            var statInfo = (StatInfoVM)sender;
            this.UpdateComparison(statInfo);
            this.UpdateBenchmarking(statInfo);
        }

        private void AddColumns(IEnumerable<IGridColumn> columns, int index = -1)
        {
            if (index == -1)
                index = _columnHeaders.Count;

            foreach (var column in columns)
            {
                _columnHeaders.Insert(index, column);
                ++index;
            }

            foreach (var dataRow in _dataRows)
            {
                var row = (IGridColumn)dataRow[RowHeaderKey];
                foreach (var column in columns)
                    dataRow[column.Key] = _StatVMGetter(row, column);
            }

            this.UpdateComparisonsAndBenchmarkingsOnAxisAdded(columns);
        }

        private void RemoveRows(IEnumerable<IGridColumn> rows)
        {
            foreach (var row in rows)
            {
                _dataRows.RemoveWhere(r => r[RowHeaderKey] == row);
                // shall we release stat value caches?
            }

            this.UpdateComparisonsAndBenchmarkingsOnAxisRemoved(rows);
        }

        private void RemoveColumns(IEnumerable<IGridColumn> columns)
        {
            foreach (var column in columns)
            {
                _columnHeaders.Remove(column);
            }

            foreach (var dataRow in _dataRows)
            {
                var row = (IGridColumn)dataRow[RowHeaderKey];
                foreach (var column in columns)
                    dataRow.Remove(column.Key);
            }

            this.UpdateComparisonsAndBenchmarkingsOnAxisRemoved(columns);
        }

        private void ReplaceRows(IEnumerable<IGridColumn> oldRows, IEnumerable<IGridColumn> newRows)
        {
            var firstReplacedItem = oldRows.First();
            var index = _dataRows.Aggregate(-1, (i, r) => r[RowHeaderKey] == firstReplacedItem ? i : i + 1);
            if (index == -1)
                throw new InvalidOperationException();

            this.RemoveRows(oldRows);
            this.AddRows(newRows, index);

            this.UpdateComparisons();
        }

        private void ReplaceColumns(IEnumerable<IGridColumn> oldColumns, IEnumerable<IGridColumn> newColumns)
        {
            var firstReplacedItem = oldColumns.First();
            var index = _columnHeaders.Aggregate(-1, (i, c) => c == firstReplacedItem ? i : i + 1);
            if (index == -1)
                throw new InvalidOperationException();

            this.RemoveColumns(oldColumns);
            this.AddColumns(newColumns, index);

            this.UpdateComparisons();
        }

        private void MoveRows(IEnumerable<IGridColumn> items, int oldStartingIndex, int newStartingIndex)
        {
            for (var i = 0; i < items.Count(); ++i)
            {
                _dataRows.Move(oldStartingIndex + i, newStartingIndex + i);
            }
        }

        private void MoveColumns(IEnumerable<IGridColumn> items, int oldStartingIndex, int newStartingIndex)
        {
            for (var i = 0; i < items.Count(); ++i)
            {
                _columnHeaders.Move(oldStartingIndex + i, newStartingIndex + i);
            }
        }

        private bool AreColumns(IEnumerable<IGridColumn> items)
        {
            if (items is IEnumerable<StatInfoVM> && this.ColumnMode.Mode == StatComparer.ColumnMode.Stats)
                return true;

            if (items is IEnumerable<TankVM> && this.ColumnMode.Mode == StatComparer.ColumnMode.Tanks)
                return true;

            return false;
        }

        private void AddRowOrColumns(IEnumerable<IGridColumn> items, int index)
        {
            if (this.AreColumns(items))
                this.AddColumns(items, index);
            else
                this.AddRows(items, index);
        }

        private void RemoveRowOrColumns(IEnumerable<IGridColumn> items)
        {
            if (this.AreColumns(items))
                this.RemoveColumns(items);
            else
                this.RemoveRows(items);
        }

        private void ReplaceRowOrColumns(IEnumerable<IGridColumn> oldItems, IEnumerable<IGridColumn> newItems)
        {
            if (this.AreColumns(oldItems))
                this.ReplaceColumns(oldItems, newItems);
            else
                this.ReplaceRows(oldItems, newItems);
        }

        private void MoveRowOrColumns(IEnumerable<IGridColumn> items, int oldStartingIndex, int newStartingIndex)
        {
            if (this.AreColumns(items))
                this.MoveColumns(items, oldStartingIndex, newStartingIndex);
            else
                this.MoveRows(items, oldStartingIndex, newStartingIndex);
        }

        private void ResetTanks()
        {
            if (this.ColumnMode.Mode == StatComparer.ColumnMode.Tanks)
                this.ResetGrid();
            else
                this.ResetValueRows();
        }

        private void ResetStats()
        {
            if (this.ColumnMode.Mode == StatComparer.ColumnMode.Stats)
                this.ResetGrid();
            else
                this.ResetValueRows();
        }

        public void NotifyColumnReordered(int oldIndex, int newIndex)
        {
            if (this.ColumnMode.Mode == StatComparer.ColumnMode.Stats)
                this.StatsManager.SelectedStats.Move(oldIndex, newIndex);
            else
                this.TanksManager.SelectedTanks.Move(oldIndex, newIndex);
        }

    }
}

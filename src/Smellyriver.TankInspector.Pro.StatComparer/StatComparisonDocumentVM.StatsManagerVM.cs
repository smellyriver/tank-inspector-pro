using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Input;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Pro.Data.Stats;
using Smellyriver.TankInspector.Pro.IO;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.Common.Wpf.Behaviors.DragDrop;
using Smellyriver.TankInspector.Common.Wpf.Input;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    partial class StatComparisonDocumentVM
    {
        public StatsManagerVM StatsManager { get; }

        public class StatsManagerVM : NotificationObject
        {
            internal static readonly string PresetsDirectory = Path.Combine(ApplicationPath.GetModuleDirectory(Assembly.GetCallingAssembly()), "Presets");
            internal const string DefaultPresetFile = "default.statspreset";
            internal static readonly string DefaultPresetFilePath = StatsManagerVM.GetPresetFilePath(DefaultPresetFile);



            static StatsManagerVM()
            {
                if (!Directory.Exists(PresetsDirectory))
                    Directory.CreateDirectory(PresetsDirectory);
            }


            private static string GetPresetFilePath(string filename)
            {
                return Path.Combine(PresetsDirectory, filename);
            }



            private readonly StatComparisonDocumentVM _owner;

            public ObservableCollection<StatCategoryVM> AvailableStatCategories { get; }
            public ObservableCollection<StatInfoVM> SelectedStats { get; }
            private readonly HashSet<StatInfoVM> _selectedStatsLookup;

            public ICommand AddStatCommand { get; private set; }
            public ICommand RemoveStatCommand { get; private set; }
            public ICommand StatMoveUpCommand { get; private set; }
            public ICommand StatMoveDownCommand { get; private set; }
            public ICommand OpenStatsPresetCommand { get; private set; }
            public ICommand SaveStatsPresetCommand { get; private set; }

            public IDropTarget SelectedStatsDropHandler { get; private set; }
            public IDropTarget AvailableStatsDropHandler { get; private set; }

            private bool _isStatPresetChanged;
            private string _statPresetFilename;

            private readonly Dictionary<string, StatCategoryVM> _statCategories;
            private readonly Dictionary<IStat, StatInfoVM> _statInfoVms;

            public StatsManagerVM(StatComparisonDocumentVM owner)
            {

                _statCategories = new Dictionary<string, StatCategoryVM>();
                _statInfoVms = new Dictionary<IStat, StatInfoVM>();
                foreach (var stat in StatsProviderManager.Instance.Stats)
                {
                    var statVm = _statInfoVms.GetOrCreate(stat, () => new StatInfoVM(stat));
                    foreach (var categoryName in stat.Categories)
                    {
                        var category = _statCategories.GetOrCreate(categoryName, () => new StatCategoryVM(categoryName));
                        category.Stats.Add(statVm);
                    }
                }

                _owner = owner;
                this.AvailableStatCategories = new ObservableCollection<StatCategoryVM>(_statCategories.Values);
                this.SelectedStats = new ObservableCollection<StatInfoVM>();
                _selectedStatsLookup = new HashSet<StatInfoVM>();

                var statsToAdd = new List<StatInfoVM>();
                foreach (var statKey in _owner.PersistentInfo.StatKeys)
                {
                    var stat = StatsProviderManager.Instance.GetStat(statKey);
                    if (stat == null)
                        this.LogWarning("stat '{0}' not found", statKey);
                    else
                        statsToAdd.Add(this.GetStatInfoVM(stat));
                }

                this.AddStatRange(statsToAdd, 0);

                this.SelectedStats.CollectionChanged += SelectedStats_CollectionChanged;

                this.AddStatCommand = new RelayCommand<object>(this.AddStat, this.CanAddStat);
                this.RemoveStatCommand = new RelayCommand<IList>(this.RemoveStat, this.CanRemoveStat);
                this.StatMoveUpCommand = new RelayCommand<IList>(this.StatMoveUp, this.CanMoveStatUp);
                this.StatMoveDownCommand = new RelayCommand<IList>(this.StatMoveDown, this.CanMoveStatDown);

                this.OpenStatsPresetCommand = new RelayCommand(this.OpenStatsPreset);
                this.SaveStatsPresetCommand = new RelayCommand(this.SaveStatsPreset);

                this.SelectedStatsDropHandler = new SelectedStatsDropHandlerImpl(this);
                this.AvailableStatsDropHandler = new AvailableStatsDropHandlerImpl(this);
            }

            private StatInfoVM GetStatInfoVM(IStat stat)
            {
                return _statInfoVms.GetOrCreate(stat, () => new StatInfoVM(stat));
            }

            private void SaveStatsPreset()
            {
                var filename = string.IsNullOrEmpty(_statPresetFilename)
                             ? StatsManagerVM.GetPresetFilePath("mypreset.statspreset")
                             : _statPresetFilename;
                if (DialogManager.Instance.ShowSaveFileDialog(this.L("stat_comparer", "save_stat_preset_dialog_title"),
                                                              ref filename,
                                                              string.Format("{0}(*.statspreset)|*.statspreset", 
                                                                            this.L("stat_comparer", "stat_preset_file_type_filter")),
                                                              defaultExtensionName: "statspreset") == true)
                {
                    try
                    {
                        using (var file = File.Create(filename))
                        {
                            var statKeys = this.SelectedStats.Select(s => s.Model.Key).ToArray();
                            this.CreateStatsPresetSerializer().WriteObject(file, statKeys);
                            _isStatPresetChanged = false;
                        }

                        _statPresetFilename = filename;
                    }
                    catch (Exception ex)
                    {
                        var message = this.L("stat_comparer", "save_stat_preset_failed_message", filename, ex.Message);
                        DialogManager.Instance.ShowMessageAsync(this.L("common", "error"), message);
                        this.LogError("failed to save stats preset from file '{0}': {1}", filename, ex.Message);
                    }
                }
            }

            private void OpenStatsPreset()
            {
                if (_isStatPresetChanged)
                {
                    DialogManager.Instance.ShowMessageAsync(this.L("stat_comparer", "save_stat_preset_prompt_title"),
                                                            this.L("stat_comparer", "save_stat_preset_prompt_message"),
                                                            MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary,
                                                            new DialogSettings
                                                            {
                                                                AffirmativeButtonText = this.L("common", "save"),
                                                                NegativeButtonText = this.L("common", "do_not_save"),
                                                                FirstAuxiliaryButtonText = this.L("common", "cancel")
                                                            })
                        .ContinueWith(t =>
                        {
                            Application.Current.Dispatcher.BeginInvoke(
                                new Action(() =>
                                {
                                    if (t.Result == MessageDialogResult.Affirmative)
                                        this.SaveStatsPreset();
                                    else if (t.Result == MessageDialogResult.Negative)
                                        this.DoOpenStatPreset();
                                }));
                        });
                }
                else
                {
                    this.DoOpenStatPreset();
                }
            }


            private void DoOpenStatPreset()
            {
                var filename = string.IsNullOrEmpty(_statPresetFilename)
                             ? DefaultPresetFilePath
                             : _statPresetFilename;
                if (DialogManager.Instance.ShowOpenFileDialog(this.L("open_stat_preset_dialog_title", "load_stat_preset_failed_message"),
                                                              ref filename,
                                                              this.L("stat_comparer", "stat_preset_file_type_filter", "(*.statspreset)|*.statspreset")) 
                                                              == true)
                {
                    this.LoadStatsPresetFile(filename);
                }
            }

            internal void LoadStatsPresetFile(string filename)
            {
                try
                {
                    using (var file = File.OpenRead(filename))
                    {
                        var statKeys = (string[])this.CreateStatsPresetSerializer().ReadObject(file);


                        var statsToAdd = new List<StatInfoVM>();
                        foreach (var statKey in statKeys)
                        {
                            var stat = StatsProviderManager.Instance.GetStat(statKey);
                            if (stat == null)
                                continue;

                            statsToAdd.Add(this.GetStatInfoVM(stat));
                        }

                        this.ClearStats();
                        this.AddStatRange(statsToAdd, 0);
                        _isStatPresetChanged = false;
                    }
                }
                catch (Exception ex)
                {
                    var message = this.L("stat_comparer", "load_stat_preset_failed_message", filename, ex.Message);
                    DialogManager.Instance.ShowMessageAsync(this.L("common", "error"), message);
                    this.LogError("failed to load stats preset from file '{0}': {1}", filename, ex.Message);                    
                }
            }

            private DataContractSerializer CreateStatsPresetSerializer()
            {
                return new DataContractSerializer(typeof(string[]), "Stats", Stat.Xmlns);
            }

            void SelectedStats_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                _owner.PersistentInfo.StatKeys = this.SelectedStats.Select(s => s.Model.Key).ToArray();

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        _owner.AddRowOrColumns(e.NewItems.Cast<StatInfoVM>(), e.NewStartingIndex);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        _owner.RemoveRowOrColumns(e.OldItems.Cast<StatInfoVM>());
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        _owner.ReplaceRowOrColumns(e.OldItems.Cast<StatInfoVM>(), e.NewItems.Cast<StatInfoVM>());
                        break;
                    case NotifyCollectionChangedAction.Move:
                        _owner.MoveRowOrColumns(e.OldItems.Cast<StatInfoVM>(), e.OldStartingIndex, e.NewStartingIndex);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        _owner.ResetStats();
                        break;
                }

                _isStatPresetChanged = true;
            }

            internal bool CanMoveStatDown(IList selectedItems)
            {
                return selectedItems.Count == 1
                    && this.SelectedStats.Last() != selectedItems[0];
            }

            internal bool CanMoveStatUp(IList selectedItems)
            {
                return selectedItems.Count == 1
                    && this.SelectedStats.First() != selectedItems[0];
            }

            internal bool CanRemoveStat(IList selectedItems)
            {
                return selectedItems.Count > 0;
            }

            internal bool CanAddStat(object selectedItem)
            {
                return selectedItem is StatInfoVM || selectedItem is StatCategoryVM;
            }

            internal void StatMoveDown(IList selectedItems)
            {
                var index = this.SelectedStats.IndexOf((StatInfoVM)selectedItems[0]);
                this.SelectedStats.Move(index, index + 1);
            }

            internal void StatMoveUp(IList selectedItems)
            {
                var index = this.SelectedStats.IndexOf((StatInfoVM)selectedItems[0]);
                this.SelectedStats.Move(index, index - 1);
            }

            internal void RemoveStat(IList selectedItems)
            {
                var stats = selectedItems.Cast<StatInfoVM>().ToArray();
                foreach (var stat in stats)
                {
                    this.SelectedStats.Remove(stat);
                    _selectedStatsLookup.Remove(stat);
                    stat.IsSelected = false;
                }

                this.UpdateCategoryVisibilities();
            }

            internal void AddStat(object selectedItem)
            {
                this.AddStat(selectedItem, -1);
            }

            internal void AddStat(object selectedItem, int index = -1)
            {
                if (index == -1)
                    index = this.SelectedStats.Count;

                var stat = selectedItem as StatInfoVM;
                if (stat != null)
                {
                    this.AddStat(stat, index);
                }
                else
                {
                    var category = selectedItem as StatCategoryVM;
                    if (category != null)
                    {
                        this.AddStatRange(category.Stats.Where(s => !s.IsSelected), index);
                    }
                }
            }

            private void RawAddStat(StatInfoVM stat, int index)
            {
                if (!_selectedStatsLookup.Contains(stat))
                {
                    this.SelectedStats.Insert(index, stat);
                    _selectedStatsLookup.Add(stat);
                    stat.IsSelected = true;
                }
            }

            private void AddStat(StatInfoVM stat, int index)
            {
                this.RawAddStat(stat, index);

                this.UpdateCategoryVisibilities();
            }

            private void AddStatRange(IEnumerable<StatInfoVM> stats, int index)
            {
                foreach (var stat in stats)
                {
                    this.RawAddStat(stat, index);
                    ++index;
                }

                this.UpdateCategoryVisibilities();
            }

            private void ClearStats()
            {
                foreach (var stat in this.SelectedStats)
                    stat.IsSelected = false;
                this.SelectedStats.Clear();
                _selectedStatsLookup.Clear();
            }

            private void UpdateCategoryVisibilities()
            {
                foreach (var category in this.AvailableStatCategories)
                    category.UpdateVisibility();
            }
        }

    }
}

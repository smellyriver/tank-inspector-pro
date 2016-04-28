using System.IO;
using System.Windows.Input;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.Menu;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    partial class StatComparisonDocumentVM
    {

        private readonly CommandBindingCollection _commandBindings;

        private void InitializeCommands()
        {
            _commandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs, this.ExecuteSaveAs));
            _commandBindings.Add(new CommandBinding(Commands.ExportCsv, this.ExecuteSaveAs));
            _commandBindings.Add(new CommandBinding(Commands.ToggleConfigPanel,
                                                    this.ExecuteToggleConfigPanel,
                                                    this.CanExecuteToggleConfigPanel));
            _commandBindings.Add(new CommandBinding(Commands.SelectBaseValueMode,
                                                    this.ExecuteSelectBaseValueMode,
                                                    this.CanExecuteSelectBaseValueMode));
            _commandBindings.Add(new CommandBinding(Commands.SelectInstanceValueMode,
                                                    this.ExecuteSelectInstanceValueMode,
                                                    this.CanExecuteSelectInstanceValueMode));
            _commandBindings.Add(new CommandBinding(Commands.SelectTanksColumnMode,
                                                    this.ExecuteSelectTanksColumnMode,
                                                    this.CanExecuteSelectTanksColumnMode));
            _commandBindings.Add(new CommandBinding(Commands.SelectStatsColumnMode,
                                                    this.ExecuteSelectStatsColumnMode,
                                                    this.CanExecuteSelectStatsColumnMode));          
        }

        private void CanExecuteSelectStatsColumnMode(object sender, CanExecuteRoutedEventArgs e)
        {
            var menuItemVm = e.Parameter as MenuItemVM;
            menuItemVm.IsChecked = this.ColumnMode.Mode == StatComparer.ColumnMode.Stats;
            e.CanExecute = true;
        }

        private void ExecuteSelectStatsColumnMode(object sender, ExecutedRoutedEventArgs e)
        {
            var menuItemVm = e.Parameter as MenuItemVM;
            this.ColumnMode = s_columnModes[(int)StatComparer.ColumnMode.Stats];
            menuItemVm.IsChecked = this.ColumnMode.Mode == StatComparer.ColumnMode.Stats;
        }

        private void CanExecuteSelectTanksColumnMode(object sender, CanExecuteRoutedEventArgs e)
        {
            var menuItemVm = e.Parameter as MenuItemVM;
            menuItemVm.IsChecked = this.ColumnMode.Mode == StatComparer.ColumnMode.Tanks;
            e.CanExecute = true;
        }

        private void ExecuteSelectTanksColumnMode(object sender, ExecutedRoutedEventArgs e)
        {
            var menuItemVm = e.Parameter as MenuItemVM;
            this.ColumnMode = s_columnModes[(int)StatComparer.ColumnMode.Tanks];
            menuItemVm.IsChecked = this.ColumnMode.Mode == StatComparer.ColumnMode.Tanks;
        }

        private void CanExecuteSelectInstanceValueMode(object sender, CanExecuteRoutedEventArgs e)
        {
            var menuItemVm = e.Parameter as MenuItemVM;
            menuItemVm.IsChecked = this.StatValueMode.Mode == Data.Stats.StatValueMode.Instance;
            e.CanExecute = true;
        }

        private void ExecuteSelectInstanceValueMode(object sender, ExecutedRoutedEventArgs e)
        {
            var menuItemVm = e.Parameter as MenuItemVM;
            this.StatValueMode = s_statValueModes[(int)Data.Stats.StatValueMode.Instance];
            menuItemVm.IsChecked = this.StatValueMode.Mode == Data.Stats.StatValueMode.Instance;
        }

        private void CanExecuteSelectBaseValueMode(object sender, CanExecuteRoutedEventArgs e)
        {
            var menuItemVm = e.Parameter as MenuItemVM;
            menuItemVm.IsChecked = this.StatValueMode.Mode == Data.Stats.StatValueMode.Base;
            e.CanExecute = true;
        }

        private void ExecuteSelectBaseValueMode(object sender, ExecutedRoutedEventArgs e)
        {
            var menuItemVm = e.Parameter as MenuItemVM;
            this.StatValueMode = s_statValueModes[(int)Data.Stats.StatValueMode.Base];
            menuItemVm.IsChecked = this.StatValueMode.Mode == Data.Stats.StatValueMode.Base;
        }

        private void CanExecuteToggleConfigPanel(object sender, CanExecuteRoutedEventArgs e)
        {
            var menuItemVm = e.Parameter as MenuItemVM;
            menuItemVm.IsChecked = this.IsEditPanelShown;
            e.CanExecute = true;
        }

        private void ExecuteToggleConfigPanel(object sender, ExecutedRoutedEventArgs e)
        {
            this.IsEditPanelShown = !this.IsEditPanelShown;
            var menuItemVm = e.Parameter as MenuItemVM;
            menuItemVm.IsChecked = this.IsEditPanelShown;
        }

        private void ExecuteSaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            var fileName = this.PersistentInfo.SaveAsPath ?? string.Format("{0}.csv", this.Title)
                           .Replace(Path.GetInvalidFileNameChars(), '-');

            var filter = string.Format("{0}(*.csv)|*.csv|{1}(*.*)|*.*",
                                       this.L("stat_comparer", "csv_file_type_filter"),
                                       this.L("common", "all_file_types_filter"));

            var result = DialogManager.Instance.ShowSaveFileDialog(title: this.L("stat_comparer", "save_as_dialog_title"),
                                                                   fileName: ref fileName,
                                                                   filter: filter,
                                                                   filterIndex: 0,
                                                                   defaultExtensionName: ".csv",
                                                                   overwritePrompt: true,
                                                                   addExtension: true,
                                                                   checkPathExists: true);
            if (result == true)
            {
                this.PersistentInfo.SaveAsPath = fileName;

                using (var writer = new StreamWriter(fileName))
                {
                    writer.Write(",");

                    foreach (var stat in this.StatsManager.SelectedStats)
                        this.CsvWriteValue(writer, stat.NameWithUnit);

                    writer.WriteLine();

                    foreach (var tank in this.TanksManager.SelectedTanks)
                    {
                        this.CsvWriteValue(writer, tank.Name);
                        foreach (var stat in this.StatsManager.SelectedStats)
                        {
                            this.CsvWriteValue(writer, _statVms[stat][tank].ValueString);
                        }

                        writer.WriteLine();
                    }
                }
            }
        }

        private void CsvWriteValue(StreamWriter writer, string value)
        {
            writer.Write(string.Format("\"{0}\",", value));
        }
    }
}

using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Smellyriver.TankInspector.Pro.Data.Stats;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Input;
using Smellyriver.TankInspector.Pro.Modularity.Modules;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Menus;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups;
using Smellyriver.TankInspector.Common.Wpf.Input;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.Menu;
using Smellyriver.TankInspector.Common.Wpf.Utilities;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    [ModuleExport("StatComparer", typeof(StatComparerModule))]
    [ExportMetadata("Guid", "C82C6C34-2DF4-4D16-B750-9B46FE78A5F3")]
    [ExportMetadata("Name", "#stat_comparer:module_name")]
    [ExportMetadata("Description", "#stat_comparer:module_description")]
    [ExportMetadata("Version", "1.0.0.0")]
    [ExportMetadata("Provider", "#stat_comparer:module_provider")]
    public class StatComparerModule : ModuleBase
    {


        internal static readonly BitmapImage CompareIcon = BitmapImageEx.LoadAsFrozen("Resources/Images/Compare_16.png");


        public override void Initialize()
        {
            DocumentServiceManager.Instance.Register(StatComparisonDocumentService.Instance);

            TankCommandManager.Instance.Register(new TankCommand(guid: StatComparisonDocumentServiceBase.AddToComparisonCommandGuid,
                                                                 name: this.L("stat_comparer", "add_to_comparison_menu_item"),
                                                                 execute: this.AddToComparison,
                                                                 priority: StatComparisonDocumentServiceBase.AddToComparisonCommandPriority,
                                                                 icon: CompareIcon));

            BulkTankCommandManager.Instance.Register(new BulkTankCommand(guid: StatComparisonDocumentServiceBase.AddToComparisonCommandGuid,
                                                                         name: this.L("stat_comparer", "add_tanks_to_comparison_menu_item"),
                                                                         execute: this.AddToComparison,
                                                                         priority: StatComparisonDocumentServiceBase.AddToComparisonCommandPriority,
                                                                         icon: CompareIcon));

            this.InitializeMenuItems();

        }

        private void InitializeMenuItems()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Commands.NewComparison, ExecuteNewComparison));

            MenuManager.Instance.Register(new MenuItemVM(this.L("stat_comparer", "new_stat_comparison_menu_item"), Commands.NewComparison)
                                          {
                                              Icon = BitmapImageEx.LoadAsFrozen("Resources/Images/Compare_16.png")
                                          }, MenuAnchor.New);

            MenuManager.Instance.Register(new MenuItemVM(this.L("stat_comparer", "export_to_csv_sheet_menu_item"), Commands.ExportCsv)
                                          {
                                              Icon = BitmapImageEx.LoadAsFrozen("Resources/Images/CSV_16.png")
                                          }, MenuAnchor.Export);


            MenuManager.Instance.Register(new MenuItemVM(this.L("stat_comparer", "configuration_panel_menu_item"), Commands.ToggleConfigPanel)
                                          {
                                              Icon = BitmapImageEx.LoadAsFrozen("Resources/Images/ConfigPanel_16.png"),
                                              IsCheckable = true,
                                          }, MenuAnchor.View);


            MenuManager.Instance.Register(new MenuItemVM(this.L("stat_comparer", "base_values_menu_item"), Commands.SelectBaseValueMode)
                                          {
                                              Icon = BitmapImageEx.LoadAsFrozen(typeof(StatVMBase).Assembly,
                                                                                "Resources/Images/BaseValue_16.png"),
                                              IsCheckable = true,
                                          }, MenuAnchor.View);

            MenuManager.Instance.Register(new MenuItemVM(this.L("stat_comparer", "instance_values_menu_item"), Commands.SelectInstanceValueMode)
                                          {
                                              Icon = BitmapImageEx.LoadAsFrozen(typeof(StatVMBase).Assembly,
                                                                                "Resources/Images/InstanceValue_16.png"),
                                              IsCheckable = true,
                                          }, MenuAnchor.View);


            MenuManager.Instance.Register(new MenuItemVM(this.L("stat_comparer", "tanks_as_columns_menu_item"), Commands.SelectTanksColumnMode)
                                          {
                                              IsCheckable = true,
                                          }, MenuAnchor.View);

            MenuManager.Instance.Register(new MenuItemVM(this.L("stat_comparer", "stats_as_columns_menu_item"), Commands.SelectStatsColumnMode)
                                          {
                                              IsCheckable = true,
                                          }, MenuAnchor.View);
        }

        private void ExecuteNewComparison(object sender, ExecutedRoutedEventArgs e)
        {
            DockingViewManager.Instance.DocumentManager.OpenDocument(StatComparisonDocumentServiceBase.CreateUri());
        }

        private void AddToComparison(TankUnikey key)
        {
            this.AddToComparison(new[] { key });
        }

        private void AddToComparison(TankUnikey[] keys)
        {

            var flyoutVm = new ComparisonSelectorFlyoutVM(keys);
            var flyout = new Flyout
            {
                Header = this.L("stat_comparer", "add_to_comparison_flyout_title"),
                IsModal = true,
                Content = new ComparisonSelectorFlyoutView { ViewModel = flyoutVm },
                CloseCommand = new RelayCommand(() => this.OnComparisonSelectorFlyoutClosed(flyoutVm)),
                Position = FlyoutPosition.Left,
                MinWidth = 200
            };
            FlyoutManager.Instance.Open(flyout);

            flyoutVm.Closed += (o, e) =>
            {
                FlyoutManager.Instance.Close(flyout);
                this.OnComparisonSelectorFlyoutClosed(flyoutVm);
            };

        }

        private void OnComparisonSelectorFlyoutClosed(ComparisonSelectorFlyoutVM flyoutVm)
        {
            if (!flyoutVm.IsProceed)
                return;

            if (flyoutVm.SelectedComparison is NewComparisonVM)
            {
                DockingViewManager.Instance.DocumentManager.OpenDocument(StatComparisonDocumentServiceBase.CreateUri())
                    .ContinueWith(t =>
                    {
                        if (t.Result == null)
                            this.LogError("failed to create stat comparison document");
                        else
                        {
                            App.BeginInvokeBackground(() =>
                            {
                                this.AddToComparisonDocument(flyoutVm.TankKeys, t.Result);
                            });
                        }
                    });
            }
            else
            {
                var comparisonVm = (AvailableComparisonVM)flyoutVm.SelectedComparison;
                this.AddToComparisonDocument(flyoutVm.TankKeys, comparisonVm.DocumentVM);
                DockingViewManager.Instance.DocumentManager.SelectDocument(comparisonVm.Document);
            }
        }

        private void AddToComparisonDocument(TankUnikey[] keys, DocumentInfo document)
        {
            if (document == null)
                return;

            var vm = document.Content.DataContext as StatComparisonDocumentVM;
            if (vm == null)
            {
                this.LogError("failed to retrieve stat comparison document view model");
                return;
            }

            this.AddToComparisonDocument(keys, vm);

        }

        private void AddToComparisonDocument(TankUnikey[] keys, StatComparisonDocumentVM documentVm)
        {
            foreach (var key in keys)
            {
                var tankVm = documentVm.TanksManager.GetTankVM(key);
                if (tankVm != null)
                {
                    if (!documentVm.TanksManager.SelectedTanks.Contains(tankVm))
                        documentVm.TanksManager.SelectedTanks.Add(tankVm);
                }
                else
                {
                    this.LogError("failed to get tank from key: {0}", keys);
                    return;
                }
            }
        }
    }

}

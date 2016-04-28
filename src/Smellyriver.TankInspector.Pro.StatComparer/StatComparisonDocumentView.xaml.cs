using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    public partial class StatComparisonDocumentView : UserControl
    {
        private static DataTemplate CreateDataTemplateWrapper(DataTemplate innerTemplate, Binding binding)
        {
            var template = new DataTemplate();
            template.VisualTree = new FrameworkElementFactory(typeof(ContentPresenter));
            template.VisualTree.SetValue(ContentPresenter.ContentTemplateProperty, innerTemplate);
            template.VisualTree.SetBinding(ContentPresenter.ContentProperty, binding);
            return template;
        }

        internal StatComparisonDocumentVM ViewModel
        {
            get { return this.DataContext as StatComparisonDocumentVM; }
            set
            {
                this.DataContext = value;
                this.UpdateColumnModeVariables();
                this.ResetColumns();
                ((INotifyCollectionChanged)this.ViewModel.ColumnHeaders).CollectionChanged += ColumnHeaders_CollectionChanged;
                this.ViewModel.ColumnModeChanged += ViewModel_ColumnModeChanged;
            }
        }


        private DataTemplate _tankTemplate;
        private DataTemplate _statInfoTemplate;
        private DataTemplate _statValueTemplate;
        private DataTemplate _columnHeaderTemplate;
        private bool _backfeedingColumnReordered;

        private int _reorderingColumnPreviousDisplayIndex;

        public StatComparisonDocumentView()
        {
            InitializeComponent();
        }


        void ViewModel_ColumnModeChanged(object sender, EventArgs e)
        {
            this.UpdateColumnModeVariables();
        }

        private void UpdateColumnModeVariables()
        {
            _tankTemplate = (DataTemplate)this.ComparisonGrid.FindResource("TankTemplate");
            _statInfoTemplate = (DataTemplate)this.ComparisonGrid.FindResource("StatInfoTemplate");
            _statValueTemplate = (DataTemplate)this.ComparisonGrid.FindResource("StatValueTemplate");
            _columnHeaderTemplate = this.ViewModel.ColumnMode.Mode == ColumnMode.Stats
                                  ? _statInfoTemplate
                                  : _tankTemplate;
        }

        private void ColumnHeaders_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.AddColumns(e.NewItems.Cast<IGridColumn>(), e.NewStartingIndex + 1);
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (!_backfeedingColumnReordered)
                        this.MoveColumns(e.OldItems.Cast<IGridColumn>(), e.OldStartingIndex + 1, e.NewStartingIndex + 1);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.RemoveColumns(e.OldItems.Cast<IGridColumn>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    this.ReplaceColumns(e.OldItems.Cast<IGridColumn>(), e.NewItems.Cast<IGridColumn>());
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.ResetColumns();
                    break;
            }
        }

        private void ReplaceColumns(IEnumerable<IGridColumn> oldColumns, IEnumerable<IGridColumn> newColumns)
        {
            var firstReplacedItem = oldColumns.First();
            var index = this.ComparisonGrid.Columns.Aggregate(-1, (i, c) => c.Header == firstReplacedItem ? i : i + 1);
            if (index == -1)
                throw new InvalidOperationException();

            this.RemoveColumns(oldColumns);
            this.AddColumns(newColumns, index);
        }

        private void RemoveColumns(IEnumerable<IGridColumn> columns)
        {
            foreach (var columnHeader in columns)
            {
                var columnToRemove = this.ComparisonGrid.Columns.FirstOrDefault(c => c.Header == columnHeader);
                if(columnToRemove!=null)
                {
                    BindingOperations.ClearAllBindings(columnToRemove);
                    this.ComparisonGrid.Columns.Remove(columnToRemove);
                }
            }
        }

        private void ClearColumns()
        {
            foreach(var column in this.ComparisonGrid.Columns)
            {
                BindingOperations.ClearAllBindings(column);
            }

            this.ComparisonGrid.Columns.Clear();
        }

        private void MoveColumns(IEnumerable<IGridColumn> columns, int oldStartingIndex, int newStartingIndex)
        {
            for (var i = 0; i < columns.Count(); ++i)
            {
                var column = this.ComparisonGrid.Columns.First(c => c.DisplayIndex == oldStartingIndex + i);
                column.DisplayIndex = newStartingIndex + i;
            }
        }

        private void AddColumns(IEnumerable<IGridColumn> columns, int index = -1)
        {
            if (index == -1)
                index = this.ComparisonGrid.Columns.Count;

            foreach (var columnHeader in columns)
            {
                var column = new DataGridTemplateColumn();
                column.Header = columnHeader;
                column.HeaderTemplate = _columnHeaderTemplate;
                column.CellTemplate = StatComparisonDocumentView.CreateDataTemplateWrapper(_statValueTemplate, new Binding(string.Format("[{0}]", columnHeader.Key)));

                this.ComparisonGrid.Columns.Insert(index, column);
                ++index;
            }
        }

        private void ResetColumns()
        {
            this.ClearColumns();

            var rowHeaderBinding = new Binding(string.Format("[{0}]", StatComparisonDocumentVM.RowHeaderKey));

            var firstColumn = new DataGridTemplateColumn();
            firstColumn.CellTemplate = this.ViewModel.ColumnMode.Mode == ColumnMode.Stats
                                     ? StatComparisonDocumentView.CreateDataTemplateWrapper(_tankTemplate, rowHeaderBinding)
                                     : StatComparisonDocumentView.CreateDataTemplateWrapper(_statInfoTemplate, rowHeaderBinding);

            firstColumn.CanUserReorder = false;

            this.ComparisonGrid.Columns.Add(firstColumn);

            this.AddColumns(this.ViewModel.ColumnHeaders);
        }

        private void SelectedStatsListItem_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var item = sender as ListBoxItem;
            if (item != null)
            {
                var stat = item.DataContext as StatInfoVM;
                if (stat != null)
                {
                    var statList = new List<StatInfoVM>();
                    statList.Add(stat);
                    if (this.ViewModel.StatsManager.RemoveStatCommand.CanExecute(statList))
                        this.ViewModel.StatsManager.RemoveStatCommand.Execute(statList);
                }
            }
        }

        private void AvailableStatsTreeViewItem_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var item = sender as TreeViewItem;
            if (item != null)
            {
                var stat = item.DataContext as StatInfoVM;
                if (stat != null)
                {
                    if (this.ViewModel.StatsManager.AddStatCommand.CanExecute(stat))
                        this.ViewModel.StatsManager.AddStatCommand.Execute(stat);
                }
            }
        }

        private void SelectedTanksListItem_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var item = sender as ListBoxItem;
            if (item != null)
            {
                var tank = item.DataContext as TankVM;
                if (tank != null)
                {
                    var tankList = new List<TankVM>();
                    tankList.Add(tank);
                    if (this.ViewModel.TanksManager.RemoveTankCommand.CanExecute(tankList))
                        this.ViewModel.TanksManager.RemoveTankCommand.Execute(tankList);
                }
            }
        }



        private void ComparisonGrid_ColumnReordered(object sender, DataGridColumnEventArgs e)
        {
            if (e.Column.DisplayIndex == 0)
                e.Column.DisplayIndex = _reorderingColumnPreviousDisplayIndex;

            if (e.Column.DisplayIndex != _reorderingColumnPreviousDisplayIndex)
            {
                _backfeedingColumnReordered = true;
                this.ViewModel.NotifyColumnReordered(_reorderingColumnPreviousDisplayIndex - 1, e.Column.DisplayIndex - 1);
                _backfeedingColumnReordered = false;
            }
        }

        private void ComparisonGrid_ColumnReordering(object sender, DataGridColumnReorderingEventArgs e)
        {
            if (e.Column.DisplayIndex == 0)
                e.Cancel = true;

            _reorderingColumnPreviousDisplayIndex = e.Column.DisplayIndex;
        }

        private void SelectedStatsList_Drop(object sender, DragEventArgs e)
        {
            var stat = e.Data.GetData(typeof(StatInfoVM)) as StatInfoVM;
            if (stat != null)
            {
                this.ViewModel.StatsManager.SelectedStats.Add(stat);
            }
        }

        private void SelectedStatsList_DragEnter(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(typeof(StatInfoVM));
            e.Effects = data == null ? DragDropEffects.None : DragDropEffects.Copy;
        }

    }
}

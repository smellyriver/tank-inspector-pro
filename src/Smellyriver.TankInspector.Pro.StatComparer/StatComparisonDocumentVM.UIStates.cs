using System;
using System.Collections.Generic;
using Smellyriver.TankInspector.Pro.Data.Stats;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    partial class StatComparisonDocumentVM
    {

        private static readonly StatValueModeVM[] s_statValueModes = StatValueModeVM.StatValueModes;
        private static readonly ColumnModeVM[] s_columnModes = ColumnModeVM.ColumnModes;

        public bool IsEditPanelShown
        {
            get { return this.PersistentInfo.IsEditPanelShown; }
            set
            {
                this.PersistentInfo.IsEditPanelShown = value;
                this.RaisePropertyChanged(() => this.IsEditPanelShown);
            }
        }

        public double EditPanelHeight
        {
            get { return this.PersistentInfo.EditPanelHeight; }
            set
            {
                this.PersistentInfo.EditPanelHeight = value;
                this.RaisePropertyChanged(() => this.EditPanelHeight);
            }
        }

        public string Title
        {
            get { return this.PersistentInfo.Title; }
            set
            {
                this.PersistentInfo.Title = value;
                this.DocumentInfo.Title = value;
                this.RaisePropertyChanged(() => this.Title);
            }
        }

        public bool IsAddStatsAndTanksTipShown
        {
            get
            {
                return this.TanksManager.SelectedTanks.Count < 2
                    || this.StatsManager.SelectedStats.Count == 0;
            }
        }

        private IEnumerable<AxisBuildingTask> _axisBuildingTasks;
        public IEnumerable<AxisBuildingTask> AxisBuildingTasks
        {
            get { return _axisBuildingTasks; }
            private set
            {
                _axisBuildingTasks = value;
                this.RaisePropertyChanged(() => this.AxisBuildingTasks);
            }
        }



        public IEnumerable<StatValueModeVM> StatValueModes { get { return s_statValueModes; } }

        public StatValueModeVM StatValueMode
        {
            get { return s_statValueModes[(int)this.PersistentInfo.ValueMode]; }
            set
            {
                if (this.PersistentInfo.ValueMode != value.Mode)
                {
                    this.PersistentInfo.ValueMode = value.Mode;
                    this.RaisePropertyChanged(() => this.StatValueMode);
                    StatComparerSettings.Default.StatValueMode = value.Mode;
                    StatComparerSettings.Default.Save();

                    this.UpdateStatsValueMode();
                    this.UpdateComparisons();
                }
            }
        }

        public event EventHandler ColumnModeChanged;

        public ColumnModeVM[] ColumnModes { get { return s_columnModes; } }

        public ColumnModeVM ColumnMode
        {
            get { return s_columnModes[(int)this.PersistentInfo.ColumnMode]; }
            set
            {
                if (value.Mode != this.PersistentInfo.ColumnMode)
                {
                    this.PersistentInfo.ColumnMode = value.Mode;

                    StatComparerSettings.Default.ColumnMode = (int)value.Mode;
                    StatComparerSettings.Default.Save();

                    this.UpdateColumnModeVariables();

                    this.RaisePropertyChanged(() => this.ColumnMode);
                    if (this.ColumnModeChanged != null)
                        this.ColumnModeChanged(this, EventArgs.Empty);

                    this.ResetGrid();
                }
            }
        }

    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Smellyriver.TankInspector.Common.Wpf;
using Smellyriver.TankInspector.Pro.Data.Tank;

namespace Smellyriver.TankInspector.Pro.Data.Stats
{
    public class StatVMBase : DependencyNotificationObject
    {


        public IStat Model { get; }
        public TankInstance Tank { get; }

        public string ShortName { get { return this.Model.ShortName; } }
        public string Description { get { return this.Model.Description; } }
        public string Unit { get { return this.Model.Unit; } }

        private StatValueMode _valueMode;
        public StatValueMode ValueMode
        {
            get { return _valueMode; }
            set
            {
                if (_valueMode != value)
                {
                    _valueMode = value;
                    this.RaisePropertyChanged(() => this.ValueMode);
                    this.RaisePropertyChanged(() => this.ValueString);
                }
            }
        }

        public string ValueString
        {
            get { return this.ValueMode == StatValueMode.Base ? this.BaseValueString : this.InstanceValueString; }
        }

        private string _instanceValueString;
        public string InstanceValueString
        {
            get { return _instanceValueString; }
            private set
            {
                _instanceValueString = value;
                this.RaisePropertyChanged(() => this.InstanceValueString);
                if (this.ValueMode == StatValueMode.Instance)
                    this.RaisePropertyChanged(() => this.ValueString);
            }
        }

        private string _baseValueString;
        public string BaseValueString
        {
            get { return _baseValueString; }
            private set
            {
                _baseValueString = value;
                this.RaisePropertyChanged(() => this.BaseValueString);
                if (this.ValueMode == StatValueMode.Base)
                    this.RaisePropertyChanged(() => this.ValueString);
            }
        }

        public string Value
        {
            get { return this.ValueMode == StatValueMode.Base ? this.BaseValue : this.InstanceValue; }
        }

        private string _instanceValue;
        public string InstanceValue
        {
            get { return _instanceValue; }
            private set
            {
                _instanceValue = value;
                this.RaisePropertyChanged(() => this.InstanceValue);
                if (this.ValueMode == StatValueMode.Instance)
                    this.RaisePropertyChanged(() => this.Value);
            }
        }

        private string _baseValue;
        public string BaseValue
        {
            get { return _baseValue; }
            private set
            {
                _baseValue = value;
                this.RaisePropertyChanged(() => this.BaseValue);
                if (this.ValueMode == StatValueMode.Base)
                    this.RaisePropertyChanged(() => this.Value);
            }
        }


        private CancellationTokenSource _updateTaskCancellationTokenSource;
        private readonly object _updateTaskCancellationTokenSourceLock = new object();

        public StatVMBase(IStat stat, TankInstance tank)
        {
            this.Model = stat;
            this.Tank = tank;

            this.InstanceValueString = this.L("common", "loading");
            this.BaseValueString = this.L("common", "loading");

            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(this.UpdateValue), DispatcherPriority.Normal);
            //this.UpdateValue();

            this.Tank.BasicConfigurationChanged += Tank_BasicConfigurationChanged;
        }

        void Tank_BasicConfigurationChanged(object sender, BasicConfigurationChangedEventArgs e)
        {
            this.UpdateValue();
        }

        private void UpdateValue()
        {
            lock (_updateTaskCancellationTokenSourceLock)
            {
                if (_updateTaskCancellationTokenSource != null && !_updateTaskCancellationTokenSource.IsCancellationRequested)
                    _updateTaskCancellationTokenSource.Cancel();
            }

            _updateTaskCancellationTokenSource = new CancellationTokenSource();
            var token = _updateTaskCancellationTokenSource.Token;

            Task.Factory.StartNew(
                new Action(() =>
                {

                    if (token.IsCancellationRequested)
                        return;

                    this.OnLoading();

                    if (token.IsCancellationRequested)
                        return;

                    this.BaseValue = this.GetBaseValue();
                    this.BaseValueString = this.Model.FormatValue(this.BaseValue);

                    if (token.IsCancellationRequested)
                        return;

                    this.InstanceValue = this.GetValue();
                    this.InstanceValueString = this.Model.FormatValue(this.InstanceValue);

                    if (token.IsCancellationRequested)
                        return;

                    this.OnLoaded();

                }), token);

        }

        private string GetBaseValue()
        {
            return this.Model.GetValue(this.Tank, this.Tank.Repository, true);
        }

        private string GetValue()
        {
            return this.Model.GetValue(this.Tank, this.Tank.Repository, false);
        }

        protected virtual void OnLoading()
        {

        }

        protected virtual void OnLoaded()
        {

        }
    }
}

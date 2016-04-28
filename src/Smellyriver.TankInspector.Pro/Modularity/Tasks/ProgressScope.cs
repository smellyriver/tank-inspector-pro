using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Smellyriver.TankInspector.Pro.Modularity.Tasks
{
    [DebuggerDisplay("{Name} (P={Progress})")]
    public abstract class ProgressScope : IProgressScope
    {
        public const double ProgressEpsilon = 0.001;

        public const double PrimaryGroupWeightValue = 1.0;

        public static IProgressScope Create(string name)
        {
            return new RootProgressScope(name);
        }

        public static bool ProgressEquals(double p1, double p2)
        {
            return Math.Abs(p1 - p2) < ProgressEpsilon;
        }

        public string Name { get; }

        public double _myProgress;

        private double _progress;
        public double Progress
        {
            get { return _progress; }
            set
            {
                if (value != _progress)
                {
                    _progress = value;
                    this.OnProgressChanged();
                }
            }
        }

        private string _primaryStatusMessage;
        public string PrimaryStatusMessage
        {
            get { return _primaryStatusMessage; }
            set
            {
                _primaryStatusMessage = value;
                this.OnStatusMessageChanged();
            }
        }

        private string _childMessage;

        private string _statusMessage;
        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                _statusMessage = value;
                this.OnStatusMessageChanged();
            }
        }

        private bool _anyChildIsIndeterminate;
        private bool _meIsIndeterminate;

        private bool _isIndeterminate;
        public bool IsIndeterminate
        {
            get { return _isIndeterminate; }
            set
            {
                if (_isIndeterminate != value)
                {
                    _isIndeterminate = value;
                    this.OnIsIndetermineChanged();
                }
            }
        }

        private readonly List<ChildProgressScope> _childScopes;
        public IEnumerable<ChildProgressScope> Children { get { return _childScopes; } }

        public double PrimaryGroupWeight { get; }

        private double _totalWeight;
        private double _childrenProgress;

        protected bool IsDisposed { get; private set; }
        protected bool IsDisposing { get; private set; }

        internal ProgressScope(string name)
        {
            this.Name = name;
            this.PrimaryGroupWeight = PrimaryGroupWeightValue;
            _childScopes = new List<ChildProgressScope>();

            _totalWeight = PrimaryGroupWeightValue;
            _childrenProgress = 0;
        }

        public IProgressScope AddChildScope(string name, double weight)
        {
            var childScope = new ChildProgressScope(name, this, weight);
            _childScopes.Add(childScope);
            _totalWeight += weight;
            this.UpdateChildrenProgress();

            return childScope;
        }

        public void ReportProgress(double progress)
        {
            if (progress < this.Progress || progress > 1)
                throw new ArgumentException("progress");

            if (this.PrimaryGroupWeight == 0)
                throw new InvalidOperationException("primary group is not defined for this scope");

            if (this.IsDisposed)
                return;

            _myProgress = progress;
            this.UpdateProgress();

            _meIsIndeterminate = false;
            this.UpdateIsIndeterime();
        }

        private void UpdateChildrenProgress()
        {
            _totalWeight = _childScopes.Sum(c => c.Weight) + this.PrimaryGroupWeight;
            if (_totalWeight == 0)
                _childrenProgress = 0;
            else
                _childrenProgress = _childScopes.Sum(c => c.Progress * c.Weight / _totalWeight);

            _anyChildIsIndeterminate = this.Children.Any(c => c.IsIndeterminate);

            this.UpdateProgress();
        }

        private void UpdateProgress()
        {
            if (_totalWeight == 0)
                this.Progress = this.IsDisposed ? 1.0 : 0.0;
            else
                this.Progress = _childrenProgress + _myProgress * this.PrimaryGroupWeight / _totalWeight;

            this.IsIndeterminate = _anyChildIsIndeterminate || _meIsIndeterminate;
        }

        private void UpdateChildrenIsIndeterime()
        {
            _anyChildIsIndeterminate = this.Children.Any(c => c.IsIndeterminate);

            this.UpdateIsIndeterime();
        }

        private void UpdateIsIndeterime()
        {
            this.IsIndeterminate = _anyChildIsIndeterminate || _meIsIndeterminate;
        }

        protected virtual void OnProgressChanged()
        {

        }

        protected virtual void OnIsIndetermineChanged()
        {

        }

        protected virtual void OnStatusMessageChanged()
        {

        }

        public void ReportStatusMessage(string message)
        {
            if (this.IsDisposed)
                return;

            this.PrimaryStatusMessage = message;
            this.UpdateStatusMessage();
        }

        private void UpdateStatusMessage()
        {
            if (string.IsNullOrWhiteSpace(_primaryStatusMessage))
                this.StatusMessage = _childMessage;
            else
                this.StatusMessage = string.Format("{0}\n{1}", _primaryStatusMessage, _childMessage);
        }

        public void ReportIsIndetermine()
        {
            if (this.IsDisposed)
                return;

            _meIsIndeterminate = true;
            this.UpdateIsIndeterime();
        }

        public virtual void Dispose()
        {
            if (this.IsDisposed || this.IsDisposing)
                return;

            this.IsDisposing = true;

            foreach (var child in this.Children)
                child.Dispose();

            this.Progress = 1.0;

            this.IsDisposing = false;
            this.IsDisposed = true;
        }

        internal void ChildReportIsIndetermine()
        {
            if (this.IsDisposing)
                return;

            this.UpdateChildrenIsIndeterime();
        }

        internal void ChildReportProgress(ChildProgressScope child)
        {
            if (this.IsDisposing)
                return;

            this.UpdateChildrenProgress();
            if (ProgressScope.ProgressEquals(child.Progress, 1.0))
            {
                child.Dispose();
            }
        }

        internal void ChildReportStatusMessage(string message)
        {
            if (this.IsDisposing)
                return;

            _childMessage = message;
            this.UpdateStatusMessage();
        }

    }
}

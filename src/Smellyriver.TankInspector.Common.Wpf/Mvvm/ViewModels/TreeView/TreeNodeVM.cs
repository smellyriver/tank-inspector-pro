using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Smellyriver.TankInspector.Common.Wpf.Utilities;

namespace Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.TreeView
{
    public abstract class TreeNodeVM : NotificationObject
    {

        protected ObservableCollection<TreeNodeVM> InternalChildren { get; private set; }
        private ReadOnlyObservableCollection<TreeNodeVM> _readOnlyChildren;

        public ReadOnlyObservableCollection<TreeNodeVM> Children
        {
            get { return _readOnlyChildren; }
        }

        public IEnumerable<TreeNodeVM> Decendants
        {
            get
            {
                if (!_isChildrenLoaded)
                {
                    this.InternalLoadChildren(true);
                }

                foreach (var child in this.InternalChildren)
                {
                    yield return child;
                    foreach (var grandChild in child.Decendants)
                        yield return grandChild;
                }
            }
        }

        public IEnumerable<TreeNodeVM> Ancestors
        {
            get
            {
                if (this.Parent != null)
                {
                    yield return this.Parent;
                    foreach (var ancestor in this.Parent.Ancestors)
                        yield return ancestor;
                }
            }
        }

        public Dispatcher Dispatcher { get; private set; }

        protected readonly TreeNodeVM DummyChild;

        public bool HasDummyChild
        {
            get { return this.Children.Count == 1 && this.Children[0] == DummyChild; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            protected set
            {
                _name = value;
                this.RaisePropertyChanged(() => this.Name);
            }
        }

        private bool _isExpanded;
        public virtual bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                this.RaisePropertyChanged(() => this.IsExpanded);

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;

                if (_isExpanded)
                    this.InternalLoadChildren();
                else if (_internalLoadChildenStrategy.IsDynamic)
                    this.InternalUnloadChildren();
            }
        }

        private bool _isSelected;
        public virtual bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                this.RaisePropertyChanged(() => this.IsSelected);
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                this.RaisePropertyChanged(() => this.IsEnabled);
            }
        }

        private Visibility _visibility = Visibility.Visible;
        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                this.RaisePropertyChanged(() => this.Visibility);
            }
        }

        private TreeNodeVM _parent;
        public TreeNodeVM Parent
        {
            get { return _parent; }
            private set
            {
                _parent = value;
                this.RaisePropertyChanged(() => this.Parent);
            }
        }

        public LoadChildenStrategy LoadChildenStrategy { get; private set; }
        private readonly InternalLoadChildenStrategy _internalLoadChildenStrategy;
        private bool _isChildrenLoaded;

        public TreeNodeVM(TreeNodeVM parent, string name, LoadChildenStrategy loadChildrenStrategy)
        {
            if (parent == null)
                this.Dispatcher = Dispatcher.CurrentDispatcher;
            else
                this.Dispatcher = parent.Dispatcher;

            this.LoadChildenStrategy = loadChildrenStrategy;
            _internalLoadChildenStrategy = new InternalLoadChildenStrategy(this.LoadChildenStrategy);

            this.Name = name;
            this.Parent = parent;

            this.Dispatcher.AutoInvoke(() =>
                {
                    this.InternalChildren = new ObservableCollection<TreeNodeVM>();
                    _readOnlyChildren = new ReadOnlyObservableCollection<TreeNodeVM>(this.InternalChildren);
                });

            if (_internalLoadChildenStrategy.IsLazy)
            {
                this.DummyChild = new DummyTreeNodeVM(this);
                this.InternalChildren.Add(DummyChild);
            }
            else
            {
                _isChildrenLoaded = true;
            }

            this.IsEnabled = true;
        }

        private void RemoveDummyChild()
        {
            if (this.HasDummyChild)
                this.InternalChildren.Remove(this.DummyChild);
        }

        public void EnsureChildrenLoaded()
        {
            this.InternalLoadChildren(true);
        }

        private void InternalLoadChildren(bool forceInstant = false)
        {
            this.Dispatcher.CheckAccess();

            if (_isChildrenLoaded)
                return;

            if (_internalLoadChildenStrategy.IsLazy)
                this.RemoveDummyChild();

            if (_internalLoadChildenStrategy.IsAsync && !forceInstant)
            {
                var loadingNode = new LoadingChildrenTreeNodeVM(this);
                this.InternalChildren.Add(loadingNode);

                var task = Task.Factory.StartNew<IEnumerable<TreeNodeVM>>(this.LoadChildren);
                task.ContinueWith(t =>
                {
                    this.Dispatcher.AutoInvoke(() =>
                    {
                        this.InternalChildren.Remove(loadingNode);
                        foreach (var child in t.Result)
                        {
                            this.InternalChildren.Add(child);
                        }
                        _isChildrenLoaded = true;
                    });
                });
            }
            else
            {
                var children = this.LoadChildren();
                this.InternalChildren.Clear();
                foreach (var child in children)
                    this.InternalChildren.Add(child);

                _isChildrenLoaded = true;
            }
        }

        protected virtual IEnumerable<TreeNodeVM> LoadChildren()
        {
            yield break;
        }

        private void InternalUnloadChildren()
        {
            this.Dispatcher.CheckAccess();

            this.UnloadChildren();

            this.InternalChildren.Clear();

            if (_internalLoadChildenStrategy.IsLazy && _internalLoadChildenStrategy.IsDynamic)
                this.InternalChildren.Add(DummyChild);

            _isChildrenLoaded = false;
        }

        protected virtual void UnloadChildren()
        {

        }

        public void ReloadChildren()
        {
            this.InternalUnloadChildren();
            this.InternalLoadChildren();
        }

        public virtual void Refresh()
        {
            this.ReloadChildren();
        }
    }
}

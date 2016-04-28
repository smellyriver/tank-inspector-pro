using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;

namespace Smellyriver.TankInspector.Common.Collections
{
    public class ViewModelMap<TModel, TViewModel> : IEnumerable<TViewModel>, IList<TViewModel>, INotifyCollectionChanged
    {

        private INotifyCollectionChanged _modelCollection;

        public INotifyCollectionChanged ModelCollection
        {
            get { return _modelCollection; }
            set
            {
                if (value != null && !(value is IEnumerable))
                    throw new ArgumentException("value must be IEnumerable", "value");

                if (_modelCollection != null)
                    _modelCollection.CollectionChanged -= ModelCollection_CollectionChanged;

                _modelCollection = value;
                this.ResetViewModelCollection();

                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        private IEnumerable<TModel> ModelCollectionEnumerable
        {
            get
            {
                if (this.ModelCollection == null)
                    return null;

                if (_modelSelector == null)
                    return this.ModelCollection as IEnumerable<TModel>;
                else
                    return this.SelectModels();
            }
        }



        private List<TViewModel> _viewModels;

        private Func<TModel, TViewModel> _viewModelFactory;

        private Func<object, TModel> _modelSelector;
        private Func<TModel, bool> _modelFilter;

        private Dictionary<TModel, int> _indexMap;
        public ViewModelMap(Func<TModel, TViewModel> viewModelFactory)
            : this(null, viewModelFactory)
        {

        }

        public ViewModelMap(INotifyCollectionChanged modelCollection, Func<TModel, TViewModel> viewModelFactory)
            : this(modelCollection, viewModelFactory, null, null)
        {

        }

        public ViewModelMap(INotifyCollectionChanged modelCollection, Func<TModel, TViewModel> viewModelFactory, Func<object, TModel> modelSelector, Func<TModel, bool> modelFilter)
        {
            _viewModelFactory = viewModelFactory;
            _viewModels = new List<TViewModel>();
            _modelSelector = modelSelector == null ? obj => (TModel)obj : modelSelector;
            _modelFilter = modelFilter == null ? x => true : modelFilter;
            _indexMap = new Dictionary<TModel, int>();
            this.ModelCollection = modelCollection;
        }

        private void ResetViewModelCollection()
        {
            _viewModels.Clear();

            if (this.ModelCollection != null)
            {
                TViewModel viewModel;
                foreach (var model in this.ModelCollectionEnumerable)
                    this.AddViewModel(model, out viewModel);

                this.ModelCollection.CollectionChanged += ModelCollection_CollectionChanged;
            }
        }

        private IEnumerable<TModel> SelectModels()
        {
            foreach (var item in (IEnumerable)this.ModelCollection)
                yield return _modelSelector(item);
        }

        private bool AddViewModel(TModel model, out TViewModel viewModel, int index = -1)
        {
            if (!_modelFilter(model))
            {
                viewModel = default(TViewModel);
                return false;
            }

            if (index == -1)
                index = _viewModels.Count;

            viewModel = _viewModelFactory(model);
            this.InsertViewModel(index, model, viewModel);

            return true;
        }

        private void InsertViewModel(int index, TModel model, TViewModel viewModel)
        {
            var increasedIndexKeys = _indexMap.Where(i => i.Value >= index).Select(i => i.Key).ToArray();

            foreach (var key in increasedIndexKeys)
            {
                ++_indexMap[key];
            }

            _viewModels.Insert(index, viewModel);
            _indexMap[model] = index;
        }

        private bool RemoveViewModel(TModel model, out TViewModel viewModel, out int index)
        {
            if (!_modelFilter(model))
            {
                viewModel = default(TViewModel);
                index = -1;
                return false;
            }

            index = _indexMap[model];
            _indexMap.Remove(model);

            var localIndex = index;
            var reducedIndexKeys = _indexMap.Where(i => i.Value > localIndex).Select(i => i.Key).ToArray();

            foreach (var key in reducedIndexKeys)
            {
                --_indexMap[key];
            }

            viewModel = _viewModels[index];
            _viewModels.RemoveAt(index);

            return true;
        }

        private void ModelCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            List<TViewModel> oldItems = null;
            List<TViewModel> newItems = null;
            var newStartingIndex = -1;
            var oldStartingIndex = -1;

            var action = e.Action;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    newItems = new List<TViewModel>();
                    newStartingIndex = _viewModels.Count;
                    for (int i = 0; i < e.NewItems.Count; ++i)
                    {
                        var model = e.NewItems[i];
                        TViewModel viewModel;
                        if (this.AddViewModel(_modelSelector(model), out viewModel))
                            newItems.Add(viewModel);
                    }

                    if (newItems.Count == 0)
                        return;

                    break;
                case NotifyCollectionChangedAction.Move:
                    oldItems = new List<TViewModel>();
                    for (int i = 0; i < e.OldItems.Count; ++i)
                    {
                        var model = (TModel)e.OldItems[i];

                        TViewModel viewModel;
                        int oldViewModelIndex;
                        if (!this.RemoveViewModel(model, out viewModel, out oldViewModelIndex))
                            continue;

                        if (oldStartingIndex == -1)
                            oldStartingIndex = oldViewModelIndex;

                        // find the previous model which has a view model in this map to decide the new index of the view model
                        var newViewModelIndex = 0;
                        var modelCollection = (IList<TModel>)this.ModelCollection;
                        for (var previousModelIndex = i + e.NewStartingIndex - 1; previousModelIndex >= 0; --previousModelIndex)
                        {
                            if (_modelFilter(modelCollection[previousModelIndex]))
                            {
                                newViewModelIndex = _indexMap[modelCollection[previousModelIndex]] + 1;
                                break;
                            }
                        }

                        this.InsertViewModel(newViewModelIndex, model, viewModel);
                        oldItems.Add(viewModel);

                        if (newStartingIndex == -1)
                            newStartingIndex = newViewModelIndex;
                    }

                    if (oldItems.Count == 0)
                        return;

                    break;
                case NotifyCollectionChangedAction.Remove:
                    oldItems = new List<TViewModel>();
                    for (int i = 0; i < e.OldItems.Count; ++i)
                    {
                        var model = (TModel)e.OldItems[i];

                        TViewModel viewModel;
                        int oldViewModelIndex;
                        if (!this.RemoveViewModel(model, out viewModel, out oldViewModelIndex))
                            continue;

                        oldItems.Add(viewModel);

                        if (oldStartingIndex == -1)
                            oldStartingIndex = oldViewModelIndex;
                    }

                    if (oldItems.Count == 0)
                        return;

                    break;
                case NotifyCollectionChangedAction.Replace:
                    newItems = new List<TViewModel>();
                    oldItems = new List<TViewModel>();
                    for (int i = 0; i < e.OldItems.Count; ++i)
                    {
                        var oldModel = (TModel)e.OldItems[i];

                        TViewModel oldViewModel;
                        int oldViewModelIndex;
                        if (this.RemoveViewModel(oldModel, out oldViewModel, out oldViewModelIndex))
                        {
                            oldItems.Add(oldViewModel);

                            if (oldStartingIndex == -1)
                                oldStartingIndex = oldViewModelIndex;
                        }

                        var newModel = (TModel)e.NewItems[i];

                        TViewModel newViewModel;
                        if (this.AddViewModel(_modelSelector(newModel), out newViewModel, oldViewModelIndex))
                        {
                            newItems.Add(oldViewModel);

                            if (newStartingIndex == -1)
                            {
                                if (oldViewModelIndex == -1)
                                    newStartingIndex = _viewModels.Count - 1;
                                else
                                    newStartingIndex = oldViewModelIndex;
                            }
                        }
                    }
                    if (newItems.Count == 0)
                    {
                        if (oldItems.Count == 0)
                            return;
                        else
                            action = NotifyCollectionChangedAction.Remove;
                    }
                    else
                    {
                        if (oldItems.Count == 0)
                            action = NotifyCollectionChangedAction.Add;
                        else
                            action = NotifyCollectionChangedAction.Replace;
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.ResetViewModelCollection();
                    break;
            }


            NotifyCollectionChangedEventArgs eventArgs;

            switch (action)
            {
                case NotifyCollectionChangedAction.Add:
                    eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems, newStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Move:
                    eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, oldItems, newStartingIndex, oldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems, oldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItems, oldItems, oldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                    break;
                default:
                    eventArgs = null;
                    break;
            }

            this.OnCollectionChanged(eventArgs);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
                this.CollectionChanged(this, e);
        }


        public int IndexOf(TViewModel item)
        {
            return _viewModels.IndexOf(item);
        }

        public void Insert(int index, TViewModel item)
        {
            throw new ReadOnlyException();
        }

        public void RemoveAt(int index)
        {
            throw new ReadOnlyException();
        }

        public TViewModel this[int index]
        {
            get { return _viewModels[index]; }
            set { throw new ReadOnlyException(); }
        }

        public void Add(TViewModel item)
        {
            throw new ReadOnlyException();
        }

        public void Clear()
        {
            throw new ReadOnlyException();
        }

        public bool Contains(TViewModel item)
        {
            return _viewModels.Contains(item);
        }

        public void CopyTo(TViewModel[] array, int arrayIndex)
        {
            _viewModels.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _viewModels.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(TViewModel item)
        {
            throw new ReadOnlyException();
        }

        public IEnumerator<TViewModel> GetEnumerator()
        {
            return _viewModels.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}

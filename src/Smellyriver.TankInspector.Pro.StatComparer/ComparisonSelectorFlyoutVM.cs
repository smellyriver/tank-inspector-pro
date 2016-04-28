using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;
using Smellyriver.TankInspector.Common.Wpf.Input;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    class ComparisonSelectorFlyoutVM
    {
        public event EventHandler Closed;

        public bool IsProceed { get; private set; }

        private readonly List<AvailableComparisonVMBase> _availableComparisons;
        public IEnumerable<AvailableComparisonVMBase> AvailableComparisons { get { return _availableComparisons; } }

        public AvailableComparisonVMBase SelectedComparison { get; set; }

        public ICommand OKCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public TankUnikey[] TankKeys { get; private set; }

        public ComparisonSelectorFlyoutVM(TankUnikey[] keys)
        {
            this.TankKeys = keys;

            _availableComparisons = new List<AvailableComparisonVMBase>();
            this.InitializeAvailableComparisons();

            this.OKCommand = new RelayCommand<AvailableComparisonVMBase>(this.Proceed, this.CanProceed);
            this.CancelCommand = new RelayCommand(this.Cancel);
        }

        private void Cancel()
        {
            this.Close();
        }

        private bool CanProceed(AvailableComparisonVMBase selected)
        {
            return this.SelectedComparison != null;
        }

        private void Proceed(AvailableComparisonVMBase selected)
        {
            this.IsProceed = true;
            this.Close();
        }

        private void Close()
        {
            if (this.Closed != null)
                this.Closed(this, EventArgs.Empty);
        }

        private void InitializeAvailableComparisons()
        {
            _availableComparisons.Add(new NewComparisonVM());

            var documents = DockingViewManager.Instance
                                              .DocumentManager
                                              .Documents
                                              .Where(d => d.Content is StatComparisonDocumentView);

            foreach (var document in documents)
            {
                _availableComparisons.Add(new AvailableComparisonVM(document));
            }

            var activeDocument = DockingViewManager.Instance.DocumentManager.ActiveDocument;
            if (activeDocument != null && activeDocument.Content is StatComparisonDocumentView)
            {
                this.SelectedComparison = _availableComparisons.OfType<AvailableComparisonVM>()
                                                               .Where(c => c.DocumentVM == ((StatComparisonDocumentView)activeDocument.Content).ViewModel)
                                                               .FirstOrDefault();
            }

            if (this.SelectedComparison == null)
                this.SelectedComparison = _availableComparisons[0];
        }
    }
}
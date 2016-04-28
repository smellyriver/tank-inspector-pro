using System;
using System.Collections.Generic;
using System.Windows.Input;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Common.Wpf.Input;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator
{
    class CreatePatchnoteFlyoutVM
    {
        public event EventHandler Closed;

        public bool IsProceed { get; private set; }
         
        private readonly List<PatchnotePairInfo> _repositoryPairs;
        public IEnumerable<PatchnotePairInfo> RepositoryPairs { get { return _repositoryPairs; } }

        public PatchnotePairInfo SelectedRepositoryPair { get; set; }

        public ICommand OKCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public CreatePatchnoteFlyoutVM(string repositoryId)
        {
            _repositoryPairs = new List<PatchnotePairInfo>();
            this.InitializeRepositoryPairs(repositoryId);

            this.OKCommand = new RelayCommand<PatchnotePairInfo>(this.Proceed, this.CanProceed);
            this.CancelCommand = new RelayCommand(this.Cancel);
        }

        private void Cancel()
        {
            this.Close();
        }

        private bool CanProceed(PatchnotePairInfo selected)
        {
            return this.SelectedRepositoryPair != null;
        }

        private void Proceed(PatchnotePairInfo selected)
        {
            this.IsProceed = true;
            this.Close();
        }

        private void Close()
        {
            if (this.Closed != null)
                this.Closed(this, EventArgs.Empty);
        }

        private void InitializeRepositoryPairs(string repositoryId)
        {
            var repository = RepositoryManager.Instance[repositoryId];
            if (repository == null)
                return;

            if (RepositoryManager.Instance.Repositories.Count < 2)
                return;



            foreach (var repository2 in RepositoryManager.Instance.Repositories)
            {
                if (repository == repository2)
                    continue;

                IRepository target, reference;
                if (repository.Version > repository2.Version)
                {
                    target = repository;
                    reference = repository2;
                }
                else
                {
                    target = repository2;
                    reference = repository;
                }

                _repositoryPairs.Add(new PatchnotePairInfo(target, reference));
            }

            this.SelectedRepositoryPair = _repositoryPairs[0];
        }
    }
}

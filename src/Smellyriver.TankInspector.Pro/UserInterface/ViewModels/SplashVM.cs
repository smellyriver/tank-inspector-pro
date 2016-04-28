using System;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shell;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Pro.Modularity.Tasks;
using Smellyriver.TankInspector.Common.Wpf.Input;

namespace Smellyriver.TankInspector.Pro.UserInterface.ViewModels
{
    class SplashVM : NotificationObject
    {
        public event EventHandler LoadingCompleted;
        public event EventHandler LoadingProgressChanged;

        private double _loadingProgress;
        public double LoadingProgress
        {
            get { return _loadingProgress; }
            set
            {
                _loadingProgress = value;
                this.RaisePropertyChanged(() => this.LoadingProgress);
                this.RaisePropertyChanged(() => this.TaskBarProgressState);
                this.RaisePropertyChanged(() => this.TaskBarProgressValue);
                if (this.LoadingProgressChanged != null)
                    this.LoadingProgressChanged(this, EventArgs.Empty);

                if (ProgressScope.ProgressEquals(_loadingProgress, 1.0))
                    if (this.LoadingCompleted != null)
                        this.LoadingCompleted(this, EventArgs.Empty);
            }
        }

        public TaskbarItemProgressState TaskBarProgressState
        {
            get
            {
                if (ProgressScope.ProgressEquals(this.LoadingProgress, 1.0))
                    return TaskbarItemProgressState.None;
                else if (this.LoadingIsIndeterminate)
                    return TaskbarItemProgressState.Indeterminate;
                else
                    return TaskbarItemProgressState.Normal;
            }
        }

        public double TaskBarProgressValue
        {
            get
            {
                return this.LoadingProgress;
            }
        }

        private bool _loadingIsIndeterminate;
        public bool LoadingIsIndeterminate
        {
            get { return _loadingIsIndeterminate; }
            set
            {
                _loadingIsIndeterminate = value;
                this.RaisePropertyChanged(() => this.LoadingIsIndeterminate);
                this.RaisePropertyChanged(() => this.TaskBarProgressState);
                this.RaisePropertyChanged(() => this.TaskBarProgressValue);
            }
        }

        private string _loadingStatusText;
        public string LoadingStatusText
        {
            get { return _loadingStatusText; }
            set
            {
                _loadingStatusText = value;
                this.RaisePropertyChanged(() => this.LoadingStatusText);
            }
        }

        private SplashViewState _viewState;
        public SplashViewState ViewState
        {
            get { return _viewState; }
            set
            {
                _viewState = value;
                this.RaisePropertyChanged(() => this.ViewState);
                this.RaisePropertyChanged(() => this.IsInLoadingViewState);
                this.RaisePropertyChanged(() => this.IsInEulaViewState);
            }
        }
        
        public bool IsInLoadingViewState { get { return this.ViewState == SplashViewState.Loading; } }
        public bool IsInEulaViewState { get { return this.ViewState == SplashViewState.Eula; } }


        private bool _isAgreedToEULA;
        public bool IsAgreedToEULA
        {
            get { return _isAgreedToEULA; }
            set
            {
                _isAgreedToEULA = value;
                this.RaisePropertyChanged(() => this.IsAgreedToEULA);
            }
        }


        public FlowDocument EulaDocument
        {
            get { return FlowDocumentHelper.LoadDocument("eula.xaml"); }
        }

        public ICommand OpenEulaPageCommand { get; private set; }
        public ICommand CloseEulaPageCommand { get; private set; }
        
        public SplashVM()
        {
            this.OpenEulaPageCommand = new RelayCommand(this.OpenEulaPage);
            this.CloseEulaPageCommand = new RelayCommand(this.CloseEulaPage);
        }
        
        private void CloseEulaPage()
        {
            this.ViewState = SplashViewState.Loading;
        }

        private void OpenEulaPage()
        {
            this.ViewState = SplashViewState.Eula;
        }
        
    }
}

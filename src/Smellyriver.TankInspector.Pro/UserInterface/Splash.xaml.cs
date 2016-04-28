using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using MahApps.Metro.Controls;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.Pro.Modularity.Tasks;
using Smellyriver.TankInspector.Pro.UserInterface.ViewModels;

namespace Smellyriver.TankInspector.Pro.UserInterface
{

    public partial class Splash : MetroWindow
    {
        internal SplashVM ViewModel
        {
            get { return this.DataContext as SplashVM; }
            private set { this.DataContext = value; }
        }

        private DoubleAnimation _progressBarAnimation;

        private bool _isShellShown;

        public Splash()
        {
            InitializeComponent();
            this.ViewModel = new SplashVM();
            this.ViewModel.LoadingProgressChanged += ViewModel_ProgressChanged;
            Bootstrapper.Current.ShellShown += Bootstrapper_ShellShown;

#if DEBUG
            this.Topmost = false;
#endif
        }

        void Bootstrapper_ShellShown(object sender, EventArgs e)
        {
            _isShellShown = true;
            Bootstrapper.Current.ShellShown -= Bootstrapper_ShellShown;
            this.Dispatcher.Invoke(new Action(this.Close));
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (!_isShellShown)
            {
                this.LogInfo("program exited by closing splash while loading");
                Environment.Exit(0);
            }
        }

        void ViewModel_ProgressChanged(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var loadingProgress = VisualTreeHelperEx.FindChild<ProgressBar>(this, "LoadingProgress");
                    if (loadingProgress == null)
                        return;

                    if (_progressBarAnimation != null)
                    {
                        _progressBarAnimation.Completed -= ProgressBarAnimation_Completed;
                    }

                    var duration = new Duration(TimeSpan.FromSeconds((this.ViewModel.LoadingProgress - loadingProgress.Value) / 0.5));
                    _progressBarAnimation = new DoubleAnimation(this.ViewModel.LoadingProgress, duration);
                    _progressBarAnimation.Completed += ProgressBarAnimation_Completed;
                    loadingProgress.BeginAnimation(RangeBase.ValueProperty, _progressBarAnimation);
                }));
        }

        void ProgressBarAnimation_Completed(object sender, EventArgs e)
        {
            if (ProgressScope.ProgressEquals(this.ViewModel.LoadingProgress, 1.0))
            {
                this.ViewModel.LoadingIsIndeterminate = true;
                this.ViewModel.LoadingStatusText = this.L("splash", "status_preparing");
            }

            _progressBarAnimation.Completed -= ProgressBarAnimation_Completed;
            _progressBarAnimation = null;
        }


    }
}

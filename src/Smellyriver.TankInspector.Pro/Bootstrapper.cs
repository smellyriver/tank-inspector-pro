using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.MefExtensions;
using Smellyriver.TankInspector.Common.Threading.Tasks;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Tasks;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Pro.UserInterface;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups;
using Localization = Smellyriver.TankInspector.Pro.Globalization.Localization;
using WpfApplication = System.Windows.Application;

namespace Smellyriver.TankInspector.Pro
{
    public partial class Bootstrapper : MefBootstrapper
    {
        internal static Bootstrapper Current { get; private set; }

        internal event EventHandler ModulesLoaded;

        private bool _isSplashCreated;
        internal bool IsShellShown { get; private set; }
        internal event EventHandler ShellShown;

        public Bootstrapper()
        {
            Bootstrapper.Current = this;

            this.InitializeLocalization();
        }

        private void InitializeLocalization()
        {
            Localization.Initialize(ApplicationSettings.Default.UICulture);
            ApplicationSettings.Default.UICulture = Localization.Instance.UICulture.Name;
            ApplicationSettings.Default.Save();
        }

        protected override ILoggerFacade CreateLogger()
        {
            return new Log4NetLogger();
        }

        protected override void ConfigureAggregateCatalog()
        {
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));
            this.AggregateCatalog.Catalogs.Add(new DirectoryCatalog(@"./Modules", "*.dll"));
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            WpfApplication.Current.MainWindow = (Shell)this.Shell;
            WpfApplication.Current.MainWindow.Show();
        }

        protected override DependencyObject CreateShell()
        {
            var shell = new Shell();

            // hack: at the first time DialogManager initializes (by importing), Shell is not created yet.
            DialogManager.Initialize(shell);

            return shell;
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            this.Container.ComposeExportedValue(this.Container);
            App.CompositionContainer = this.Container;
        }

        private void OnModulesLoaded()
        {
            this.ModulesLoaded?.Invoke(this, EventArgs.Empty);
        }

        private void DoInitializeModules(IProgressScope progress)
        {
            progress.ReportStatusMessage("Initializing modules...");
            base.InitializeModules();
        }

        protected override void InitializeModules()
        {
            Core.Initialize(new CoreSupport());

            LoadingManager.Instance.Enqueue(ActionTask.Create("initializeModules", this.DoInitializeModules));
            LoadingManager.Instance.Enqueue(new RepositoryManager.LoadRepositoriesTask(RepositoryManager.Instance));
            LoadingManager.Instance.Enqueue(
                new DockingViewManager.DocumentManagerImpl.RestoreDocumentsTask(
                    (DockingViewManager.DocumentManagerImpl)DockingViewManager.Instance.DocumentManager));

            LoadingManager.Instance.Enqueue(ActionTask.Create("onModulesLoaded", this.OnModulesLoaded));

            var shell = (Shell)this.Shell;
            shell.Hide();

            this.CreateSplash();

            while (!_isSplashCreated)
                Thread.Sleep(100);

            LoadingManager.Instance.BeginLoad()
                          .ContinueWith(
                          t =>
                          {
                              App.BeginInvokeBackground(this.ShowShell);
                          });
        }

        private void ShowShell()
        {
            ((Shell)this.Shell).Show();
            this.IsShellShown = true;
            this.ShellShown?.Invoke(this, EventArgs.Empty);
        }

        private Task CreateSplash()
        {
            return STATaskFactory.StartNew(() =>
                {

                    var splash = new Splash();
                    splash.Closed += (s, e) =>
                       Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);

                    splash.Show();

                    LoadingManager.Instance.SplashVM = splash.ViewModel;

                    _isSplashCreated = true;
                    Dispatcher.Run();
                });
        }
    }
}

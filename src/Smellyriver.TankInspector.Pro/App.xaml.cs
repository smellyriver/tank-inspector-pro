using System;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using log4net;
using WpfApplication = System.Windows.Application;

namespace Smellyriver.TankInspector.Pro
{
    public partial class App : WpfApplication
    {
        private static readonly ILog log = SafeLog.GetLogger("App");

        public static DispatcherOperation BeginInvokeBackground(Action action)
        {
            return Application.Current.Dispatcher.BeginInvoke(action, DispatcherPriority.Background);
        }

        public static CompositionContainer CompositionContainer { get; internal set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //Theme.Initialize();

            Program.InitializeUnhandledExceptionHandler();

            var versionLog = SafeLog.GetLogger("Version");
            versionLog.Info(Assembly.GetExecutingAssembly().GetName().Version.ToString());

            Program.LogHardwareInfo();

#if (DEBUG)
            App.RunInDebugMode();
#else
            try
            {
                App.RunInReleaseMode();
            }
            catch(ReflectionTypeLoadException ex)
            {
                log.Fatal("exception(s) occurred while loading reflection type", ex);
                for (var i = 0; i < ex.LoaderExceptions.Length; ++i)
                    log.Fatal(i, ex.LoaderExceptions[i]);
            }
#endif
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

        private static void RunInDebugMode()
        {
            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }

        private static void RunInReleaseMode()
        {
            AppDomain.CurrentDomain.UnhandledException += App.AppDomainUnhandledException;
            try
            {
                var bootstrapper = new Bootstrapper();
                bootstrapper.Run();
            }
            catch (Exception ex)
            {
                App.HandleException(ex);
            }
        }


        private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            App.HandleException(e.ExceptionObject as Exception);
        }

        private static void HandleException(Exception ex)
        {
            if (ex == null)
                return;

            log.Fatal("an unhandled exception occurred: ", ex);

            Program.LogLastFirstChanceException();

            MessageBox.Show("An unhandled exception occurred, application will be terminated.");

            Environment.Exit(1);
        }

        public static string GetLocalizedString(string key)
        {
            return key;
        }
    }
}

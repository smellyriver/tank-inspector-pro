using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using MahApps.Metro.Controls.Dialogs;
using Ookii.Dialogs.Wpf;
using Smellyriver.TankInspector.Common.Threading.Tasks;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.Pro.Modularity.Tasks;
using MetroMessageDialogResult = MahApps.Metro.Controls.Dialogs.MessageDialogResult;
using MetroMessageDialogStyle = MahApps.Metro.Controls.Dialogs.MessageDialogStyle;

namespace Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups
{
    public class DialogManager
    {
        public static DialogManager Instance { get; private set; }

        internal static void Initialize(Shell shell)
        {
            DialogManager.Instance = new DialogManager(shell);
        }

        private readonly Shell _shell;

        private DialogManager(Shell shell)
        {
            _shell = shell;
        }

        public Task<IProgressDialogController> ShowProgressAsync(string title,
                                                                 string message,
                                                                 bool isCancellable = false,
                                                                 DialogSettings settings = null)
        {

            return DialogManager.ShowExternalProgressDialogAsync(title, message, isCancellable);

            //    return _shell.Dispatcher.AutoInvoke(
            //        () => _shell.ShowProgressAsync(title, message, isCancellable, settings == null ? null : settings.ToMetroDialogSettings())
            //                    .ContinueWith<IProgressDialogController>(t => new MetroProgressDialogControllerAdapter(t.Result)));
        }

        public Task ShowProgressAsync(string title,
                                      ITask task,
                                      bool isCancellable = false,
                                      DialogSettings settings = null)
        {
            settings = settings ?? new DialogSettings() { UseAnimationOnHide = false, UseAnimationOnShow = false };
            return this.ShowProgressAsync(title, string.Empty, isCancellable, settings)
                .ContinueWith(t =>
                {
                    Thread.Sleep(500);
                    DialogManager.AssignTask(t.Result, task);
                });

        }

        private static Task<IProgressDialogController> ShowExternalProgressDialogAsync(string title, string message, bool isCancellable)
        {
            IProgressDialogController controller = null;
            STATaskFactory.StartNew(() =>
            {
                var shellHandle = (IntPtr)Application.Current.Dispatcher.Invoke(new Func<IntPtr>(
                        () => new WindowInteropHelper(Application.Current.MainWindow).Handle));

                var dialog = new ProgressDialog(shellHandle);
                var shellRect = (Rect)Application.Current.Dispatcher.Invoke(new Func<Rect>(
                        () => new Rect(Application.Current.MainWindow.WindowState == WindowState.Maximized ? 0 : Application.Current.MainWindow.Left,
                                       Application.Current.MainWindow.WindowState == WindowState.Maximized ? 0 : Application.Current.MainWindow.Top,
                                       Application.Current.MainWindow.ActualWidth,
                                       Application.Current.MainWindow.ActualHeight)));

                dialog.Left = shellRect.Left;
                dialog.Top = shellRect.Top;
                dialog.Width = shellRect.Width;
                dialog.Height = shellRect.Height;

                dialog.Title = title;
                dialog.Message = message;
                dialog.IsCancellable = isCancellable;

                dialog.Closed += (s, e) =>
                   Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);

                dialog.AnimateAndShow();

                controller = dialog.ProgressDialogController;

                Dispatcher.Run();
            });

            return Task.Factory.StartNew(() =>
                {
                    while (controller == null)
                        Thread.Sleep(10);

                    return controller;
                });
        }

        public static void AssignTask(IProgressDialogController progress, ITask task)
        {
            var taskManager = new TaskManager(task.Name);
            taskManager.Enqueue(task);

            var progressChangedHandler = new EventHandler((o, e) =>
            {
                progress.SetProgress(taskManager.Progress);
                if (ProgressScope.ProgressEquals(taskManager.Progress, 1.0))
                    progress.CloseAsync();
            });
            var statusMessageChangedHandler = new EventHandler((o, e) => progress.SetMessage(taskManager.StatusMessage));
            var isIndetermineChangedHandler = new EventHandler((o, e) => { if (taskManager.IsIndeterminate)progress.SetIndeterminate(); });

            taskManager.ProgressChanged += progressChangedHandler;
            taskManager.StatusMessageChanged += statusMessageChangedHandler;
            taskManager.IsIndetermineChanged += isIndetermineChangedHandler;

            taskManager.Process().Wait();

            taskManager.ProgressChanged -= progressChangedHandler;
            taskManager.StatusMessageChanged -= statusMessageChangedHandler;
            taskManager.IsIndetermineChanged -= isIndetermineChangedHandler;
        }



        public Task<MessageDialogResult> ShowMessageAsync(string title,
                                                          string message,
                                                          MessageDialogStyle style = MessageDialogStyle.Affirmative,
                                                          DialogSettings settings = null)
        {
            return _shell.Dispatcher.AutoInvoke(
                () => _shell.ShowMessageAsync(title, message, (MetroMessageDialogStyle)style, settings == null ? null : settings.ToMetroDialogSettings())
                                .ContinueWith<MessageDialogResult>(r => (MessageDialogResult)r.Result));
        }

        public Task<MessageDialogResult> ShowYesNoMessageAsync(string title,
                                                               string message,
                                                               DialogSettings settings = null)
        {
            if (settings == null)
                settings = new DialogSettings();
            settings.AffirmativeButtonText = "Yes";
            settings.NegativeButtonText = "No";
            return this.ShowMessageAsync(title, message, MessageDialogStyle.AffirmativeAndNegative, settings);
        }

        public Task<MessageDialogResult> ShowYesNoCancelMessageAsync(string title,
                                                                     string message,
                                                                     DialogSettings settings = null)
        {
            if (settings == null)
                settings = new DialogSettings();
            settings.AffirmativeButtonText = this.L("common", "yes");
            settings.NegativeButtonText = this.L("common", "no");
            settings.FirstAuxiliaryButtonText = this.L("common", "cancel");
            return this.ShowMessageAsync(title, message, MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, settings);
        }

        public Task<string> ShowInputDialogAsync(string title,
                                                 string message,
                                                 InputDialogSettings settings = null)
        {
            if (settings == null)
                settings = new InputDialogSettings();

            return _shell.Dispatcher.AutoInvoke(
                () => _shell.ShowInputAsync(title, message, settings.ToMetroDialogSettings()));
        }

        public bool? ShowSaveFileDialog(string title,
                                        ref string fileName,
                                        string filter,
                                        int filterIndex = 0,
                                        string defaultExtensionName = null,
                                        bool overwritePrompt = true,
                                        bool addExtension = true,
                                        bool checkPathExists = true)
        {
            var dialog = new VistaSaveFileDialog();
            dialog.Title = title;
            dialog.FileName = fileName;
            dialog.CheckPathExists = checkPathExists;
            dialog.AddExtension = addExtension;
            dialog.DefaultExt = defaultExtensionName;
            dialog.OverwritePrompt = true;
            dialog.Filter = filter;
            dialog.FilterIndex = filterIndex;
            var result = dialog.ShowDialog(Application.Current.MainWindow);
            fileName = dialog.FileName;
            return result;
        }


        public bool? ShowOpenFileDialog(string title,
                                        ref string fileName,
                                        string filter,
                                        int filterIndex = 0)
        {
            var dialog = new VistaOpenFileDialog();
            dialog.Title = title;
            dialog.FileName = fileName;
            dialog.CheckFileExists = true;
            dialog.Filter = filter;
            dialog.FilterIndex = filterIndex;
            dialog.Multiselect = false;
            var result = dialog.ShowDialog(Application.Current.MainWindow);
            fileName = dialog.FileName;
            return result;
        }

        public bool? ShowOpenFileDialog(string title,
                                        out string[] fileNames,
                                        string filter,
                                        int filterIndex = 0)
        {
            var dialog = new VistaOpenFileDialog();
            dialog.Title = title;
            dialog.CheckFileExists = true;
            dialog.Filter = filter;
            dialog.FilterIndex = filterIndex;
            dialog.Multiselect = true;
            var result = dialog.ShowDialog(Application.Current.MainWindow);
            fileNames = dialog.FileNames;
            return result;
        }

        public bool? ShowFolderBrowserDialog(string description,
                                             ref string path,
                                             bool showNewFolderButton = true)
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.Description = description;
            dialog.SelectedPath = path;
            dialog.ShowNewFolderButton = showNewFolderButton;
            var result = dialog.ShowDialog();
            path = dialog.SelectedPath;
            return result;
        }
    }
}

using System;
using System.Threading.Tasks;

namespace Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups
{
    internal class ProgressDialogController : IProgressDialogController
    {

        private T Invoke<T>(Func<T> func)
        {
            return (T)this.Dialog.Dispatcher.Invoke(func);
        }

        private void Invoke(Action action)
        {
            this.Dialog.Dispatcher.Invoke(action);
        }

        public bool IsOpen { get { return this.Invoke(() => this.Dialog.IsOpen); } }

        public bool IsCanceled { get { return this.Invoke(() => this.Dialog.IsCanceled); } }

        public void SetIndeterminate()
        {
            this.Invoke(() => this.Dialog.IsProgressIndeterminate = true);
        }

        public void SetCancellable(bool value)
        {
            this.Invoke(() => this.Dialog.IsCancellable = value);
        }

        public void SetProgress(double value)
        {
            this.Invoke(() =>
                {
                    this.Dialog.IsProgressIndeterminate = false;
                    this.Dialog.Progress = value;
                });
        }

        public void SetMessage(string message)
        {
            this.Invoke(() => this.Dialog.Message = message);
        }

        public void SetTitle(string title)
        {
            this.Invoke(() => this.Dialog.Title = title);
        }

        public Task CloseAsync()
        {
            return this.Dialog.AnimateAndClose();
        }

        public ProgressDialog Dialog { get; }

        public ProgressDialogController(ProgressDialog dialog)
        {
            this.Dialog = dialog;
        }
    }
}

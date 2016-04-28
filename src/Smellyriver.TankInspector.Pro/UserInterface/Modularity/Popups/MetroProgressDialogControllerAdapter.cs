using System.Threading.Tasks;
using MetroProgressDialogController = MahApps.Metro.Controls.Dialogs.ProgressDialogController;

namespace Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups
{
    class MetroProgressDialogControllerAdapter : IProgressDialogController
    {

        private readonly MetroProgressDialogController _controller;
        public bool IsOpen
        {
            get { return _controller.IsOpen; }
        }

        public bool IsCanceled
        {
            get { return _controller.IsCanceled; }
        }

        public void SetIndeterminate()
        {
            _controller.SetIndeterminate();
        }

        public void SetCancellable(bool value)
        {
            _controller.SetCancelable(value);
        }

        public void SetProgress(double value)
        {
            _controller.SetProgress(value);
        }

        public void SetMessage(string message)
        {
            _controller.SetMessage(message);
        }

        public void SetTitle(string title)
        {
            _controller.SetTitle(title);
        }

        public Task CloseAsync()
        {
            return _controller.CloseAsync();
        }

        public MetroProgressDialogControllerAdapter(MetroProgressDialogController controller)
        {
            _controller = controller;
        }
    }
}

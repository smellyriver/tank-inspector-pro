using System.Threading.Tasks;

namespace Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups
{
    public interface IProgressDialogController
    {
        bool IsOpen { get; }
        bool IsCanceled { get; }
        void SetIndeterminate();
        void SetCancellable(bool value);
        void SetProgress(double value);
        void SetMessage(string message);
        void SetTitle(string title);
        Task CloseAsync();
    }
}

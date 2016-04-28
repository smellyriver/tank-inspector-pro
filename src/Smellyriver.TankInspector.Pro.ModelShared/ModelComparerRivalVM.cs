using System.Windows.Media;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.UserInterface;

namespace Smellyriver.TankInspector.Pro.ModelShared
{
    public sealed class ModelComparerRivalVM
    {

        public string Name { get { return _tank.Tank.Name; } }
        public ImageSource Icon { get { return _tank.Repository.GetMarker(); } }

        private readonly TankInstance _tank;
        public TankInstance Model { get { return _tank; } }

        public ModelComparerRivalVM(TankInstance tank)
        {
            _tank = tank;
        }
    }
}

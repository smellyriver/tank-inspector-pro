using Smellyriver.TankInspector.Pro.Data.Tank.Scripting;
using Smellyriver.TankInspector.Pro.Repository;
using TankEntity = Smellyriver.TankInspector.Pro.Data.Entities.Tank;

namespace Smellyriver.TankInspector.Pro.Data.Tank
{
    public abstract class ConfigurationBase : CoreNotificationObject, IConfiguration
    {
        public TankEntity Tank { get; private set; }
        public IRepository Repository { get; private set; }
        internal ScriptHost ScriptHost { get; private set; }


        internal ConfigurationBase(IRepository repository, TankEntity tank, ScriptHost scriptHost)
        {
            this.Tank = tank;
            this.Repository = repository;
            this.ScriptHost = scriptHost;
        }
    }
}

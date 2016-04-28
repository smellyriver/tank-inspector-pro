namespace Smellyriver.TankInspector.Pro.Data.Tank.Scripting
{
    abstract class Script
    {
        public abstract int Priority { get; }
        public abstract void Execute(ScriptingContext context);
    }
}

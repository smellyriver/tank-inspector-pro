using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smellyriver.TankInspector.Pro.Data.Tank.Scripting
{
    class CamouflageNetScript : AccessoryScript
    {
        public CamouflageNetScript(IXQueryable accessory) : base(accessory) {}

        public override void Execute(ScriptingContext context)
        {
            base.Execute(context);
            context.SetValue(this.Accessory["script/@name"], "invisibilityIncrementFactor", 1);
        }
    }
}

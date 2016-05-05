using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Smellyriver.TankInspector.Pro.Data.Tank.Scripting
{
    class CamouflageScript : Script
    {
        public override int Priority
        {
            get { return 10; }
        }

        public static CamouflageScript Create(IXQueryable camouflage)
        {
            if (camouflage == null)
                return null;
            
            return new CamouflageScript(camouflage);
        }

        private readonly IXQueryable _camouflage;

        public IXQueryable Camouflage
        {
            get { return _camouflage; }
        }

        public CamouflageScript(IXQueryable camouflage)
        {
            _camouflage = camouflage;
        }

        public override void Execute(ScriptingContext context)
        {
            double value;
            if (double.TryParse(this.Camouflage["invisibilityFactor"],
                                NumberStyles.Float | NumberStyles.AllowThousands,
                                CultureInfo.InvariantCulture,
                                out value))
            {
                context.SetValue("camouflage", "camouflagePaintFactor", value);
            }
        }


    }
}

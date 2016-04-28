using System;
using System.Globalization;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public class Module : Component, IModuleUnlockTarget
    {
        public static bool TryParseModuleType(string moduleType, out Type type)
        {
            switch (moduleType)
            {
                case "gun":
                    type = typeof(Gun);
                    return true;
                case "turret":
                    type = typeof(Turret);
                    return true;
                case "engine":
                    type = typeof(Engine);
                    return true;
                case "radio":
                    type = typeof(Radio);
                    return true;
                case "chassis":
                    type = typeof(Chassis);
                    return true;
                default:
                    type = null;
                    return false;
            }
        }

        public static Type ParseModuleType(string moduleType)
        {
            switch (moduleType)
            {
                case "gun":
                    return typeof(Gun);
                case "turret":
                    return typeof(Turret);
                case "engine":
                    return typeof(Engine);
                case "radio":
                    return typeof(Radio);
                case "chassis":
                    return typeof(Chassis);
                default:
                    throw new NotSupportedException();
            }
        }

        public static Module Create(IXQueryable data)
        {
            switch(data.Name)
            {
                case "gun":
                    return new Gun(data);
                case "turret":
                    return new Turret(data);
                case "engine":
                    return new Engine(data);
                case "radio":
                    return new Radio(data);
                case "chassis":
                    return new Chassis(data);
                default:
                    throw new NotSupportedException();
            }
        }

        public new string Name
        {
            get { return this["userString"]; }
        }


        public int Tier
        {
            get { return this.QueryInt("level"); }
        }

        public double Weight
        {
            get { return this.QueryDouble("weight"); }
        }

        public double UnlockExperience
        {
            get
            {
                int experience;
                if (int.TryParse(this["experience"], NumberStyles.Integer, CultureInfo.InvariantCulture, out experience))
                    return experience;

                return 0.0;
            }
        }

        public Module(IXQueryable moduleData)
            : base(moduleData)
        {
           
        }


    }
}

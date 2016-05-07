using System;
using System.Diagnostics;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    [DebuggerDisplay("{Name} ({ElementName})")]
    public abstract class TankObject : XQueryableWrapper, IEquatable<TankObject>
    {

        public string Key
        {
            get { return this["@key"]; }
        }

        public string ElementName
        {
            get { return base.Name; }
        }

        public new string Name
        {
            get { return this["userString"]; }
        }

        public int Price
        {
            get { return this.QueryInt("price"); }
        }

        public Currency Currency
        {
            get
            {
                return this["price/@currency"] == "gold"
                     ? Currency.Gold
                     : Currency.Credit;
            }
        }


        protected TankObject(IXQueryable data)
            : base(data)
        {

        }

        public bool Equals(TankObject other)
        {
            if (other == null)
                return false;

            return this.GetType() == other.GetType() && this.Key == other.Key;
        }

        public override bool Equals(object obj)
        {
            if (obj is TankObject)
                return this.Equals((TankObject)obj);

            return false;
        }

        public override int GetHashCode()
        {
            return (this.GetType().GetHashCode() << 16) + this.Key.GetHashCode();
        }
    }
}

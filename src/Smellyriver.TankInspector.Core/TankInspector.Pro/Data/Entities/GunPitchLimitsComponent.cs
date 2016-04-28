using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Smellyriver.TankInspector.Pro.Repository.Versioning;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public class GunPitchLimitsComponent : XQueryableWrapper
    {
        public class PitchData : XQueryableWrapper
        {

            public double Angle { get { return this.QueryDouble("@angle"); } }
            public double Limit { get { return this.QueryDouble("@value"); } }
            public PitchData(IXQueryable data)
                : base(data)
            { }
        }

        private GunPitchLimitsComponent.PitchData[] _data;
        public PitchData[] Data
        {
            get
            {
                return _data ?? (_data = this.QueryMany("data/pitchData").Select(d => new PitchData(d)).ToArray());
            }
        }

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public bool HasSingularValue
        {
            get { return this.Data.Length == 1 || (this.Data.Length == 2 && this.Data[0].Limit == this.Data[1].Limit); }
        }

        public GunPitchLimitsComponent(IXQueryable data)
            : base(data)
        {
        }

        public double GetValue(double degree)
        {
            degree %= 360;

            if (degree < 0)
                degree += 360;

            degree /= 360;

            return this.InterpolatePitchLimit(degree);
        }


        private double InterpolatePitchLimit(double progress)
        {
            double fromLimit = 0, toLimit = 0, fromAngle = 0, toAngle = 0;

            for (var i = 0; i < this.Data.Length - 1; ++i)
            {
                fromAngle = this.Data[i].Angle;
                toAngle = this.Data[i + 1].Angle;
                if (fromAngle <= progress && toAngle > progress)
                {
                    fromLimit = this.Data[i].Limit;
                    toLimit = this.Data[i + 1].Limit;
                    break;
                }
            }

            return fromLimit + (progress - fromAngle) * (toLimit - fromLimit) / (toAngle - fromAngle);
        }

        public double GetMaxValue()
        {
            var sign = Math.Sign(this.Data[0].Limit);
            return this.Data.Max(d => d.Limit * sign) * sign;
        }

        public double GetMinValue()
        {
            var sign = Math.Sign(this.Data[0].Limit);
            return this.Data.Min(d => d.Limit * sign) * sign;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.Data.Stats
{
    [DataContract(Namespace = Stat.Xmlns)]
    public abstract class XPathStat<TStat> : XPathStat
    {

        private class ConvertedComparer : IComparer
        {
            private readonly XPathStat<TStat> _owner;
            public ConvertedComparer(XPathStat<TStat> owner)
            {
                _owner = owner;
            }

            public int Compare(object x, object y)
            {
                var tx = _owner.Convert((string)x);
                var ty = _owner.Convert((string)y);
                return _owner.Comparer.Compare(tx, ty);
            }
        }



        protected virtual IComparer<TStat> Comparer
        {
            get { return Comparer<TStat>.Default; }
        }

        protected override IComparer InternalComparer
        {
            get { return new ConvertedComparer(this); }
        }

        [DataMember]
        public string FormatString { get; private set; }

        public XPathStat(string key,
                         string name,
                         string shortName,
                         string description,
                         string unit,
                         string xpath,
                         string baseValueXPath,
                         string showCondition,
                         CompareStrategy compareStrategy,
                         string formatString)
            : base(key, name, shortName, description, unit, xpath, baseValueXPath, showCondition, compareStrategy)
        {
            this.FormatString = string.IsNullOrEmpty(formatString) ? "{0}" : formatString;
        }

        public XPathStat(string key,
                         string name,
                         string description,
                         string xpath,
                         string showCondition,
                         CompareStrategy compareStrategy,
                         string formatString)
            : this(key, name, null, description, null, xpath, xpath, showCondition, compareStrategy, formatString)
        {

        }

        public override string GetValue(IXQueryable queryable, IRepository repository, bool isBaseValue)
        {

            var xpath = isBaseValue ? this.BaseValueXPath : this.XPath;
            try
            {
                return this.GetValue(queryable, repository, xpath);
            }
            catch (Exception ex)
            {
                Core.Support.LogError(this,
                                                          string.Format(
                                                                        "error evaluating stat xpath(key='{0}', xpath='{1}'): {2}",
                                                                        this.Key,
                                                                        xpath,
                                                                        ex.Message));
                var defaultValue = default(TStat);
                return defaultValue == null ? null : defaultValue.ToString();
            }

        }

        protected virtual string GetValue(IXQueryable queryable, IRepository repository, string xpath)
        {
            return queryable.QueryValue(xpath);
        }

        public override string FormatValue(string value)
        {
            this.FormatString = string.IsNullOrEmpty(this.FormatString) ? "{0}" : this.FormatString;
            return string.Format(this.FormatString, this.Convert(value));
        }

        protected virtual TStat Convert(string value)
        {
            return (TStat)TypeDescriptor.GetConverter(typeof(TStat)).ConvertFrom(value);
        }

        public override double? GetDifferenceRatio(string value1, string value2)
        {
            return this.InternalGetDifferenceRatio(this.Convert(value1), this.Convert(value2));
        }

        protected abstract double? InternalGetDifferenceRatio(TStat value1, TStat value2);

        public override double? GetDifference(string value1, string value2)
        {
            return this.InternalGetDifference(this.Convert(value1), this.Convert(value2));
        }

        protected abstract double? InternalGetDifference(TStat value1, TStat value2);

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.FormatString = string.IsNullOrEmpty(this.FormatString) ? "{0}" : this.FormatString;
        }

    }
}

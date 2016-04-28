using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

using Smellyriver.TankInspector.Pro.Data.Stats.Specialized;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.Data.Stats
{
    [DataContract(Name = "Stat", Namespace = Stat.Xmlns)]
    [KnownType(typeof(StringStat))]
    [KnownType(typeof(NumberStat))]
    [KnownType(typeof(BooleanStat))]
    [KnownType(typeof(CurrencyStat))]
    [DebuggerDisplay("XPathStat:{Key}")]
    public abstract class XPathStat : IStat
    {
        [DataMember(IsRequired = true)]
        public string Key { get; private set; }

        [DataMember(Name = "ShortName", IsRequired = false)]
        private string _shortName;
        public string ShortName
        {
            get { return string.IsNullOrEmpty(_shortName) ? this.Name : _shortName; }
            set { _shortName = value; }
        }

        [DataMember(IsRequired = false, Name = "Categories")]
        private string _plainCategories;
        public string[] Categories { get; protected set; }

        [DataMember(IsRequired = true)]
        public string Name { get; private set; }

        [DataMember(IsRequired = false)]
        public string Description { get; set; }

        [DataMember(IsRequired = false)]
        public CompareStrategy CompareStrategy { get; set; }

        [DataMember(IsRequired = true)]
        public string XPath { get; internal set; }

        [DataMember(Name = "BaseValueXPath", IsRequired = false)]
        private string _baseValueXPath;
        public string BaseValueXPath
        {
            get { return string.IsNullOrEmpty(_baseValueXPath) ? this.XPath : _baseValueXPath; }
            internal set { _baseValueXPath = value; }
        }

        internal string RawBaseValueXPath
        {
            get { return _baseValueXPath; }
        }

        [DataMember(Name = "ComparisonXPath", IsRequired = false)]
        private string _comparisonXPath;
        public string ComparisonXPath
        {
            get { return string.IsNullOrEmpty(_comparisonXPath) ? this.XPath : _baseValueXPath; }
            internal set { _comparisonXPath = value; }
        }

        internal string RawComparisonXPath
        {
            get { return _comparisonXPath; }
        }

        [DataMember(Name = "ShowCondition", IsRequired = false)]
        private string _showCondition;
        public string ShowConditionXPath
        {
            get { return _showCondition; }
            internal set { _showCondition = value; }
        }

        [DataMember(IsRequired = false)]
        public BenchmarkThreshold BenchmarkThreshold { get; private set; }


        [DataMember(IsRequired = false)]
        public string Unit { get; private set; }

        bool IStat.IsStatic
        {
            get { return true; }
        }

        IComparer IStat.Comparer
        {
            get { return this.InternalComparer; }
        }

        protected abstract IComparer InternalComparer { get; }

        internal XPathStat(string key,
                           string name,
                           string shortName,
                           string description,
                           string unit,
                           string xpath,
                           string baseValueXPath,
                           string showCondition,
                           CompareStrategy compareStrategy)
        {
            this.Key = key;
            this.Name = name;
            this.ShortName = shortName;
            this.Description = description;
            this.Unit = unit;
            this.XPath = xpath;
            this.BaseValueXPath = baseValueXPath;
            this.ShowConditionXPath = showCondition;
            this.CompareStrategy = compareStrategy;
            this.BenchmarkThreshold = BenchmarkThreshold.Default;
        }


        public abstract string GetValue(IXQueryable queryable, IRepository repository, bool isBaseValue);
        public abstract string FormatValue(string value);
        public abstract double? GetDifferenceRatio(string value1, string value2);
        public abstract double? GetDifference(string value1, string value2);

        public bool ShouldShowFor(IXQueryable queryable, IRepository repository)
        {
            if (string.IsNullOrEmpty(this.ShowConditionXPath))
                return true;

            var queryResult = queryable.QueryValue(this.ShowConditionXPath);
            if (string.IsNullOrEmpty(queryResult) || queryResult == "False")
                return false;

            return true;
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            _plainCategories = this.Categories == null
                             ? null
                             : string.Join(", ", this.Categories);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.Categories = string.IsNullOrEmpty(_plainCategories)
                            ? new string[0]
                            : _plainCategories.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(Core.Support.Localize)
                            .ToArray();
            
            if (this.BenchmarkThreshold == null)
                this.BenchmarkThreshold = BenchmarkThreshold.Default;

            this.Name = Core.Support.Localize(this.Name);
            if (_shortName != null)
                _shortName = Core.Support.Localize(_shortName);

            if (this.Description != null)
                this.Description = Core.Support.Localize(this.Description);

            if (this.Unit != null)
                this.Unit = Core.Support.Localize(this.Unit);

        }
    }



}

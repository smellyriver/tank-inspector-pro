using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Data.Stats;
using Smellyriver.TankInspector.Pro.Networking.Livestat;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.LivestatProvider
{
    class Livestat : IStat
    {
        private class StringDoubleComparer : IComparer
        {
            private static readonly IComparer s_doubleComparer = Comparer<double>.Default;
            public int Compare(object x, object y)
            {
                if (x == null && y == null)
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                var dx = double.Parse((string)x);
                var dy = double.Parse((string)y);
                return s_doubleComparer.Compare(dx, dy);
            }
        }

        private static readonly IComparer s_comparer;

        static Livestat()
        {
            s_comparer = new StringDoubleComparer();
        }

        public string Key { get; }
        public string[] Categories { get; }
        public string ShortName { get; }

        public string Name { get; }

        public BenchmarkThreshold BenchmarkThreshold { get; }

        public string Unit { get; }

        public string Description { get; }

        public CompareStrategy CompareStrategy { get; }

        public IComparer Comparer { get { return s_comparer; } }

        public Func<LivestatData, double> ValueRetriever { get; }

        private readonly string _formatString;

        bool IStat.IsStatic
        {
            get { return false; }
        }
        public Livestat(string key,
                        Func<LivestatData, double> valueRetriever,
                        string[] categories,
                        string name,
                        string shortName,
                        string description,
                        string unit,
                        BenchmarkThreshold benchmarkThreshold,
                        CompareStrategy compareStrategy,
                        string formatString)
        {
            this.Key = key;
            this.ValueRetriever = valueRetriever;
            this.Categories = categories.Select(c => this.L(c)).ToArray();
            this.Name = this.L(name);
            this.ShortName = this.L(shortName);
            this.Description = this.L(description);
            this.Unit = this.L(unit);
            this.CompareStrategy = compareStrategy;
            this.BenchmarkThreshold = benchmarkThreshold;
            _formatString = formatString;
        }

        public string GetValue(IXQueryable queryable, IRepository repository, bool isBaseValue)
        {
            var livestat = LivestatDatabase.Instance[queryable, repository];
            return livestat == null ? null : this.ValueRetriever(livestat).ToString();
        }

        public string FormatValue(string value)
        {
            double doubleValue;
            if (double.TryParse(value, out doubleValue))
                return string.Format(_formatString, doubleValue);

            return null;
        }

        public bool ShouldShowFor(IXQueryable queryable, IRepository repository)
        {
            return true;
        }

        public double? GetDifferenceRatio(string value1, string value2)
        {
            if (value1 == null || value2 == null)
                return null;

            var d1 = double.Parse(value1);
            var d2 = double.Parse(value2);

            return (d1 - d2) / d2;
        }

        public double? GetDifference(string value1, string value2)
        {
            if (value1 == null || value2 == null)
                return null;

            var d1 = double.Parse(value1);
            var d2 = double.Parse(value2);

            return d1 - d2;
        }
    }
}

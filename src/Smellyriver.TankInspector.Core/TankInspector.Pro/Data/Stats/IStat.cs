using System.Collections;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.Data.Stats
{
    public interface IStat
    {
        bool IsStatic { get; }
        string Key { get; }
        string ShortName { get; }
        string Name { get; }
        string Unit { get; }
        string Description { get; }
        string[] Categories { get; }
        CompareStrategy CompareStrategy { get; }
        IComparer Comparer { get; }
        bool IsBenchmarkable { get; }
        BenchmarkThreshold BenchmarkThreshold { get; }
        string GetValue(IXQueryable queryable, IRepository repository, bool isBaseValue);
        double? GetBenchmarkValue(IXQueryable queryable, IRepository repository, bool isBaseValue);
        string FormatValue(string value);
        bool ShouldShowFor(IXQueryable queryable, IRepository repository);
        double? GetDifferenceRatio(string value1, string value2);
        double? GetDifference(string value1, string value2);
    }
}

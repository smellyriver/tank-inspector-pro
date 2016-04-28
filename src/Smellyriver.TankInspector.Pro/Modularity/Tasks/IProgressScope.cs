using System;

namespace Smellyriver.TankInspector.Pro.Modularity.Tasks
{
    public interface IProgressScope : IDisposable
    {
        double Progress { get; }
        bool IsIndeterminate { get; }
        string StatusMessage { get; }
        void ReportIsIndetermine();
        void ReportProgress(double progress);
        void ReportStatusMessage(string message);
        IProgressScope AddChildScope(string name, double weight);
    }
}

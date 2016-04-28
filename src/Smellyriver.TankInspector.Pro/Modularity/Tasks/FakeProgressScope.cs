namespace Smellyriver.TankInspector.Pro.Modularity.Tasks
{
    sealed class FakeProgressScope : IProgressScope
    {
        public static FakeProgressScope Instance { get; private set; }

        static FakeProgressScope()
        {
            FakeProgressScope.Instance = new FakeProgressScope();
        }

        private FakeProgressScope()
        {

        }

        public void Dispose()
        {
            
        }

        public double Progress
        {
            get { return 0.0; }
        }

        public bool IsIndeterminate
        {
            get { return false; }
        }

        public string StatusMessage
        {
            get { return null; }
        }

        public void ReportIsIndetermine()
        {
            
        }

        public void ReportProgress(double progress)
        {
            
        }

        public void ReportStatusMessage(string message)
        {
            
        }

        public IProgressScope AddChildScope(string name, double weight)
        {
            return FakeProgressScope.Instance;
        }




    }
}

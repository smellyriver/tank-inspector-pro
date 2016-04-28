using System.Collections.Generic;
using System.Windows.Documents;

namespace Smellyriver.TankInspector.Pro.StatsInspector
{
    class CreateDocumentResult
    {
        public FlowDocument Document { get; private set; }
        public IEnumerable<StatVM> StatVms { get; private set; }

        public CreateDocumentResult(FlowDocument document, IEnumerable<StatVM> statVms)
        {
            this.Document = document;
            this.StatVms = statVms;
        }
    }
}

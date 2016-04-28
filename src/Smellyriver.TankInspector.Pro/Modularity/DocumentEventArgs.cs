using System;

namespace Smellyriver.TankInspector.Pro.Modularity
{
    public class DocumentEventArgs : EventArgs
    {
        public DocumentInfo Document { get; private set; }

        public DocumentEventArgs(DocumentInfo document)
        {
            this.Document = document;
        }
    }
}

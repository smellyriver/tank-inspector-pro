using System;
using Smellyriver.TankInspector.Pro.Modularity.Tasks;

namespace Smellyriver.TankInspector.Pro.Modularity
{
    public class CreateDocumentTask : ICreateDocumentTask
    {
        public static ICreateDocumentTask FromFactory(Func<DocumentInfo> factory)
        {
            return new CreateDocumentTask(p => factory());
        }

        public static ICreateDocumentTask FromFactory(Func<IProgressScope, DocumentInfo> factory)
        {
            return new CreateDocumentTask(factory);
        }

        public DocumentInfo DocumentInfo { get; private set; }

        public string Name
        {
            get { return "createDocument"; }
        }

        private Func<IProgressScope, DocumentInfo> _factory;

        public CreateDocumentTask(Func<IProgressScope, DocumentInfo> factory)
        {
            _factory = factory;
        }

        public void Initialize(ITaskNode taskScope)
        {

        }

        public void PreProcess(IProgressScope progress)
        {
            this.DocumentInfo = _factory(progress);
        }

        public void PostProcess(IProgressScope progress)
        {

        }

        public void RunSynchronized(IProgressScope progress)
        {
            this.DocumentInfo = _factory(progress);
        }
    }
}

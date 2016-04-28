using Smellyriver.TankInspector.Pro.Modularity.Tasks;

namespace Smellyriver.TankInspector.Pro.Modularity
{
    public interface ICreateDocumentTask :  ITask
    {
        DocumentInfo DocumentInfo { get; }
    }
}

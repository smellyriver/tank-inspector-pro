using System;

namespace Smellyriver.TankInspector.Pro.Modularity
{
    public interface IDocumentService
    {
        Guid Guid { get; }

        string[] SupportedSchemes { get; }

        ICreateDocumentTask CreateCreateDocumentTask(Uri uri, Guid guid, string persistentInfo);

    }
}

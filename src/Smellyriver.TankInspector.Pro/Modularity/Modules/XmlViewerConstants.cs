using System;

namespace Smellyriver.TankInspector.Pro.Modularity.Modules
{
    public static class XmlViewerConstants
    {
        public static readonly Guid ViewTankXmlCommandGuid = Guid.Parse("353C2BB4-7CD3-42EA-93C5-8A12942B13BC");
        public const int ViewTankXmlCommandPriority = 0;

        public static readonly Guid ViewTankInstanceXmlSnapshotCommandGuid = Guid.Parse("E168F18F-E4A5-4637-B80F-6A2D9539A964");
        public const int ViewTankInstanceXmlSnapshotCommandPriority = 0;

        public const string XmlScheme = "xml";
    }
}

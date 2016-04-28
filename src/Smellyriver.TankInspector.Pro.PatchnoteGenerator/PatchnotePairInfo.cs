using System.Windows.Media;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Pro.UserInterface;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator
{
    class PatchnotePairInfo
    {
        public ImageSource ReferenceRepositoryIcon { get; private set; }
        public string ReferenceRepositoryName { get; private set; }
        public ImageSource TargetRepositoryIcon { get; private set; }
        public string TargetRepositoryName { get; private set; }

        public IRepository Target { get; private set; }
        public IRepository Reference { get; private set; }

        public PatchnotePairInfo(IRepository target, IRepository reference)
        {
            this.Target = target;
            this.Reference = reference;

            this.TargetRepositoryIcon = target.GetMarker();
            this.TargetRepositoryName = RepositoryHelper.GetRepositoryDisplayName(target);
            this.ReferenceRepositoryIcon = reference.GetMarker();
            this.ReferenceRepositoryName = RepositoryHelper.GetRepositoryDisplayName(reference);
        }
    }
}

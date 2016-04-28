using System.Windows.Media;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Repository;
using WpfColor = System.Windows.Media.Color;

namespace Smellyriver.TankInspector.Pro.CustomizationConfigurator
{
    class CamouflageVM
    {

        private readonly Camouflage _model;
        public Camouflage Model
        {
            get { return _model; }
        }

        public ImageSource PreviewImage { get; private set; }

        public WpfColor[] Colors { get { return _model.GetColors(); } }
        public string Description { get { return _model.Description; } }

        public string GroupName
        {
            get { return _model.Group.Name; }
        }

        public CamouflageVM(Camouflage camouflage, WpfColor baseColor, IRepository repository)
        {
            _model = camouflage;
            this.PreviewImage = CamouflagePreview.GetCamouflagePreview(camouflage, baseColor, repository);
        }
    }
}

using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.IO;
using Smellyriver.TankInspector.Pro.ModelShared;

namespace Smellyriver.TankInspector.Pro.ModelInspector
{
    class ModelViewVM : ModelVMBase
    {

        private ModelType _modelType;
        public ModelType ModelType
        {
            get { return _modelType; }
            internal set
            {
                _modelType = value;
                this.RaisePropertyChanged(() => this.ModelType);
            }
        }

        private TextureMode _textureMode;
        public TextureMode TextureMode
        {
            get { return _textureMode; }
            internal set
            {
                _textureMode = value;
                this.RaisePropertyChanged(() => this.TextureMode);
            }
        }

        private FileSource _fileSource;
        public FileSource FileSource
        {
            get { return _fileSource; }
            internal set
            {
                _fileSource = value;
                this.RaisePropertyChanged(() => this.FileSource);
            }
        }

        private bool _showCamouflage;
        public bool ShowCamouflage
        {
            get { return _showCamouflage; }
            internal set
            {
                _showCamouflage = value;
                this.RaisePropertyChanged(() => this.ShowCamouflage);
            }
        }


        public ModelViewVM(TankInstance tankInstance)
            :base(tankInstance)
        {
            
            this.ShowCamouflage = true;
            this.FileSource = FileSource.Package;
            this.TextureMode = TextureMode.Normal;
            this.ModelType = ModelType.Undamaged;
        }
    }
}

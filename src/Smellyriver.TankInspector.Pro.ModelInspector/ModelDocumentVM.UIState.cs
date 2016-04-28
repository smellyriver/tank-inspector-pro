using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.IO;

namespace Smellyriver.TankInspector.Pro.ModelInspector
{
    partial class ModelDocumentVM
    {
        public bool UseUndamagedModel
        {
            get { return this.ModelView.ModelType == ModelType.Undamaged; }
            set
            {
                if (value == true)
                    this.SetModelType(ModelType.Undamaged);
            }
        }

        public bool UseDestroyedModel
        {
            get { return this.ModelView.ModelType == ModelType.Destroyed; }
            set
            {
                if (value == true)
                    this.SetModelType(ModelType.Destroyed);
            }
        }

        public bool UseExplodedModel
        {
            get { return this.ModelView.ModelType == ModelType.Exploded; }
            set
            {
                if (value == true)
                    this.SetModelType(ModelType.Exploded);
            }
        }

        private void SetModelType(ModelType modelType)
        {
            this.ModelView.ModelType = modelType;
            this.PersistentInfo.ModelType = modelType;
            this.RaisePropertyChanged(() => this.UseExplodedModel);
            this.RaisePropertyChanged(() => this.UseDestroyedModel);
            this.RaisePropertyChanged(() => this.UseUndamagedModel);
        }

        public bool UseOfficialTexture
        {
            get { return this.ModelView.FileSource == FileSource.Package; }
            set
            {
                var textureSource = value ? FileSource.Package : FileSource.ModFolder;
                this.ModelView.FileSource = textureSource;
                this.PersistentInfo.TextureSource = textureSource;
                this.RaisePropertyChanged(() => this.UseOfficialTexture);
                this.RaisePropertyChanged(() => this.UseModTexture);
            }
        }

        public bool UseModTexture
        {
            get { return this.ModelView.FileSource == FileSource.ModFolder; }
            set
            {
                var textureSource = value ? FileSource.ModFolder : FileSource.Package;
                this.ModelView.FileSource = textureSource;
                this.PersistentInfo.TextureSource = textureSource;
                this.RaisePropertyChanged(() => this.UseModTexture);
                this.RaisePropertyChanged(() => this.UseOfficialTexture);
            }
        }

        public bool UseNormalTexture
        {
            get { return this.ModelView.TextureMode == TextureMode.Normal; }
            set
            {
                var textureMode = value ? TextureMode.Normal : TextureMode.Grid;
                this.ModelView.TextureMode = textureMode;
                this.PersistentInfo.TextureMode = textureMode;
                this.RaisePropertyChanged(() => this.UseNormalTexture);
                this.RaisePropertyChanged(() => this.UseGridTexture);
            }
        }

        public bool UseGridTexture
        {
            get { return this.ModelView.TextureMode == TextureMode.Grid; }
            set
            {
                var textureMode = value ? TextureMode.Grid : TextureMode.Normal;
                this.ModelView.TextureMode = textureMode;
                this.PersistentInfo.TextureMode = textureMode;
                this.RaisePropertyChanged(() => this.UseGridTexture);
                this.RaisePropertyChanged(() => this.UseNormalTexture);
            }
        }

        public bool ShowCamouflage
        {
            get { return this.ModelView.ShowCamouflage; }
            set
            {
                this.ModelView.ShowCamouflage = value;
                this.PersistentInfo.ShowCamouflage = value;
                this.RaisePropertyChanged(() => this.ShowCamouflage);
            }
        }


    }
}

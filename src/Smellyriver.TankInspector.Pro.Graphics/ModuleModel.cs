using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SharpDX.Direct3D9;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.IO;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Graphics.Frameworks;
using Smellyriver.TankInspector.Pro.Graphics.Scene;
using Smellyriver.TankInspector.Pro.IO;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.Graphics
{
    public class ModuleModel : NotificationObject, IDisposable
    {
        public Task LoadingTask { get; set; }

        public ModelVisual Visual { get; private set; }

        public ModelPrimitive Primitives { get; private set; }

        private ModuleMesh _mesh;

        internal ModuleMesh Mesh
        {
            get { return _mesh; }
            private set { _mesh = value; }
        }


        private Module _module;

        public LocalGameClient GameClient
        {
            get { return this.TankInstance.Repository as LocalGameClient; }
        }

        public string ModelName
        {
            get { return _module.Name; }
        }

        public IArmoredObject ArmoredObject
        {
            get { return (IArmoredObject)_module; }
        }

        public IModelObject ModelObject
        {
            get { return (IModelObject)_module; }
        }

        public TankInstance TankInstance { get; }


        private CancellationTokenSource _loadCancellationTokenSource;

        public CamouflageInfo Camouflage { get; private set; }

        public ModuleModel(TankInstance tankInstance, Module module)
        {
            if (!(tankInstance.Repository is LocalGameClient))
                throw new ArgumentException("tankInstance");

            this.TankInstance = tankInstance;

            this.Camouflage = new CamouflageInfo(tankInstance, module);

            _module = module;
        }


        public static Stream OpenVisualFile(string modelFile, IFileLocator fileLocator)
        {
            var visualFile = Path.ChangeExtension(modelFile, ".visual");    // pre 10.0
            Stream stream;
            if (fileLocator.TryOpenRead(visualFile, out stream))
                return stream;

            visualFile = Path.ChangeExtension(modelFile, ".visual_processed"); // post 10.0
            return fileLocator.OpenRead(visualFile);
        }

        public static Stream OpenPrimitiveFile(string modelFile, IFileLocator fileLocator)
        {
            var primitiveFile = Path.ChangeExtension(modelFile, ".primitives"); // pre 10.0

            Stream stream;
            if (fileLocator.TryOpenRead(primitiveFile, out stream))
                return stream;

            primitiveFile = Path.ChangeExtension(modelFile, ".primitives_processed"); // post 10.0
            return fileLocator.OpenRead(primitiveFile);
        }

        public void Load(ModelType modelType,
                         IFileLocator fileLocator,
                         Device renderDevice,
                         GraphicsSettings graphicsSettings)
        {
            if (_loadCancellationTokenSource != null)
                _loadCancellationTokenSource.Cancel();

            _loadCancellationTokenSource = new CancellationTokenSource();

            var modelPath = this.ModelObject.GetModelPath(modelType);

            this.LoadingTask = Task.Factory.StartNew(() =>
            {
                try
                {
                    using (Diagnostics.PotentialExceptionRegion)
                    {
                        using (var visualStream = OpenVisualFile(modelPath, fileLocator))
                        {
                            this.Visual = ModelVisual.ReadFrom(visualStream);
                            foreach (var rendetSet in Visual.RenderSets)
                            {
                                foreach (var group in rendetSet.Geometry.ModelPrimitiveGroups)
                                {
                                    var material = group.Value.Material;
                                    material.ShowArmor = modelType == ModelType.Collision;
                                    material.Armor = this.ArmoredObject.GetArmorGroup(material.Identifier);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    this.LogInfo("exception occurred when loading {0} visual: {1}", modelType, e.Message);
                }
            },
                                                     _loadCancellationTokenSource.Token)
                                   .ContinueWith(_ =>
                                   {
                                       try
                                       {
                                           using (Diagnostics.PotentialExceptionRegion)
                                           {
                                               using (var primitivesStream = OpenPrimitiveFile(modelPath, fileLocator))
                                               {
                                                   this.Primitives = ModelPrimitive.ReadFrom(primitivesStream,
                                                                                             Visual,
                                                                                             modelType == ModelType.Collision);
                                               }
                                           }
                                       }
                                       catch (Exception e)
                                       {
                                           this.LogInfo("exception occurred when loading {0} primitives: {1}",
                                                        modelType,
                                                        e.Message);
                                       }
                                   },
                                                 _loadCancellationTokenSource.Token)
                                   .ContinueWith(_ =>
                                   {
                                       var oldMesh = this.Mesh;
                                       this.Mesh = new ModuleMesh(this, fileLocator, renderDevice, graphicsSettings);
                                       Disposer.RemoveAndDispose(ref oldMesh);
                                   },
                                                 _loadCancellationTokenSource.Token);
        }

        public void Render(Effect effect, ref int triangleCount)
        {
            if (this.Mesh != null)
                this.Mesh.Render(effect, ref triangleCount);
        }

        private string GetPackagePath(string filename, string declaredPackagePath)
        {
            string actualPackagePath;
            if (!PackageStream.IsFileExisted(declaredPackagePath, filename))
            {
                actualPackagePath = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(declaredPackagePath)),
                                                 "shared_content.pkg");
            }
            else
                actualPackagePath = declaredPackagePath;
            return actualPackagePath;
        }

        private void ReadPaths(out string modelPath, out string packagePath, ModelType type)
        {
            modelPath = this.ModelObject.GetModelPath(type)
                            .Replace("//", "/");

            if (!string.IsNullOrEmpty(modelPath))
                modelPath = modelPath.Substring(0, modelPath.LastIndexOf('.'));

            var pkgNameInfo = modelPath.Split('/');
            packagePath = Path.Combine(this.GameClient.RootPath,
                                       "res",
                                       "packages",
                                       string.Format("{0}_{1}.pkg", pkgNameInfo[0], pkgNameInfo[1]));
        }


        public void Dispose()
        {
            Disposer.RemoveAndDispose(ref _mesh);
        }
    }
}

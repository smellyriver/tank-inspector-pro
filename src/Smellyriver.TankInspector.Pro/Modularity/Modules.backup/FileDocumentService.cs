using log4net;
using Smellyriver.TankInspector.IO;
using Smellyriver.TankInspector.Pro.IO;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Features;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Pro.UserInterface;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Dialogs;
using Smellyriver.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Smellyriver.TankInspector.Pro.Modularity.Modules
{
    [Export(typeof(IFileDocumentService))]
    [Export(typeof(IAutoRegisteredDocumentService))]
    public partial class FileDocumentService : IFileDocumentService, IDocumentService, IAutoRegisteredDocumentService
    {
        public const string StreamScheme = "stream";

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);

        private readonly Dictionary<string, IFileViewerService> _services;
        private readonly Dictionary<string, FileTypeInfo> _fileTypes;

        private readonly Dictionary<string, StreamInfo> _registeredStreams;

        Guid IDocumentService.Guid
        {
            get { return Guid.Parse("EC2BBA62-B596-45B3-B664-5C8DE08D722D"); }
        }

        string[] IDocumentService.SupportedSchemes
        {
            get { return new[] { Uri.UriSchemeFile, FileDocumentService.StreamScheme }; }
        }

        [Import]
        private IDialogManager DialogManager { get; set; }

        [Import]
        private IRepositoryManager RepositoryManager { get; set; }

        public FileDocumentService()
        {
            _services = new Dictionary<string, IFileViewerService>();
            _fileTypes = new Dictionary<string, FileTypeInfo>();
            _registeredStreams = new Dictionary<string, StreamInfo>();
        }

        public Uri CreateTemporaryStreamUri(string category,
                                            string extensionName,
                                            string path,
                                            Stream stream,
                                            string title,
                                            string description,
                                            IRepository ownerRepository)
        {
            var viewer = this.GetFileViewerServiceOrNotifyMissing(extensionName);
            if (viewer == null)
                return null;

            var key = string.Format("{0}.{1}/{2}", category, extensionName, Uri.EscapeDataString(path));
            StreamInfo streamInfo;

            if (_registeredStreams.TryGetValue(key, out streamInfo))
            {
                return streamInfo.Uri;
            }

            streamInfo = new StreamInfo()
            {
                FileType = extensionName,
                Title = title,
                StreamReference = new WeakReference<Stream>(stream),
                Description = description,
                OwnerRepository = ownerRepository
            };

            _registeredStreams.Add(key, streamInfo);
            var uri = new Uri(string.Format("{0}://{1}",
                                            FileDocumentService.StreamScheme,
                                            key),
                              UriKind.Absolute);

            streamInfo.Uri = uri;

            return uri;
        }

        private static string NormalizeExtensionName(string extensionName)
        {
            if (extensionName == null)
                return null;

            if (extensionName[0] == '.')
                extensionName = extensionName.Substring(1);

            return extensionName.ToLower();
        }

        public IFileViewerService GetFileViewerService(string extensionName)
        {
            extensionName = NormalizeExtensionName(extensionName);
            IFileViewerService service;
            if (_services.TryGetValue(extensionName, out service))
                return service;

            log.WarnFormat("no FileViewerService for extension name '{0}' found", extensionName);

            return null;
        }

        public IFileViewerService GetFileViewerServiceOrNotifyMissing(string extensionName)
        {
            var viewer = this.GetFileViewerService(extensionName);
            if (viewer == null)
                this.DialogManager.ShowMessageAsync("Missing Viewer Module",
                    string.Format("The required file viewer module for '{0}' file is not installed or missing.", extensionName));

            return viewer;
        }


        public ImageSource GetIconSource(string extensionName)
        {
            extensionName = NormalizeExtensionName(extensionName);
            FileTypeInfo fileTypeInfo;
            if (_fileTypes.TryGetValue(extensionName, out fileTypeInfo))
                return fileTypeInfo.IconSource;

            return null;
        }

        public void RegisterViewer(IFileViewerService service)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            if (service.SupportedFileTypes == null || service.SupportedFileTypes.Length == 0)
                throw new ArgumentException("service", "one or more supported file types required");

            foreach (var fileType in service.SupportedFileTypes)
            {
                var normalizedExtensionName = NormalizeExtensionName(fileType.ExtensionName);
                log.InfoFormat("registering FileViewerService '{0}' for file type '{1}'", service.Name, normalizedExtensionName);

                IFileViewerService existedService;
                if (_services.TryGetValue(normalizedExtensionName, out existedService))
                    log.WarnFormat("a FileViewerService '{0}' of file type '{1}' is already existed, it will be replaced",
                                   existedService.Name,
                                   normalizedExtensionName);

                _services[normalizedExtensionName] = service;
                _fileTypes[normalizedExtensionName] = fileType;
            }
        }

        public bool HasFileViewerService(string extensionName)
        {
            return _services.ContainsKey(NormalizeExtensionName(extensionName));
        }

        private DocumentInfo CreateDocument(Guid guid, Uri uri, IFileViewerService fileViewerService)
        {
            IFeature[] features;
            var viewer = fileViewerService.CreateViewer(uri.LocalPath, out features);
            var document = new DocumentInfo(guid: guid,
                                            repositoryId: this.RepositoryManager.FindOwner(uri.LocalPath).ID,
                                            uri: uri,
                                            title: UnifiedPath.GetFileName(uri.LocalPath),
                                            content: viewer,
                                            features: features)
                                            {
                                                Description = uri.LocalPath,
                                                IconSource = this.RepositoryManager.FindOwner(uri.LocalPath).GetMarker()
                                            };

            return document;
        }

        DocumentInfo IDocumentService.CreateDocument(Uri uri, Guid guid, string persistentInfo)
        {
            if (uri.Scheme == Uri.UriSchemeFile)
            {
                var path = uri.LocalPath;
                if (path == null || !UnifiedFile.Exists(path))
                    return null;

                var fileViewer = this.GetFileViewerService(UnifiedPath.GetExtension(path));
                if (fileViewer == null)
                    return null;

                return this.CreateDocument(guid, uri, fileViewer);

            }
            else if (uri.Scheme == FileDocumentService.StreamScheme)
            {

                var streamId = uri.Host + uri.PathAndQuery;
                StreamInfo streamInfo;
                if (!_registeredStreams.TryGetValue(streamId, out streamInfo))
                {
                    log.WarnFormat("no registered stream with ID '{0}' found", streamId);
                    return null;
                }

                _registeredStreams.Remove(streamId);

                if (!streamInfo.StreamReference.IsAlive)
                {
                    log.WarnFormat("the registered stream with ID '{0}' is no more alive", streamId);
                    return null;
                }

                var fileViewer = this.GetFileViewerServiceOrNotifyMissing(streamInfo.FileType);
                if (fileViewer == null)
                {
                    return null;
                }


                IFeature[] features;

                var viewer = fileViewer.CreateViewer(streamInfo.StreamReference.Target, out features);
                var document = DocumentInfo.CreateTemporary(streamInfo.OwnerRepository.ID, streamInfo.Title, uri, viewer, features);
                document.Description = streamInfo.Description;

                return document;
            }

            return null;
        }


    }
}

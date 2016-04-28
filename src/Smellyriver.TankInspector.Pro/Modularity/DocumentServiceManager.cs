using System;
using System.Collections.Generic;
using Smellyriver.TankInspector.Pro.InternalModules;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups;

namespace Smellyriver.TankInspector.Pro.Modularity
{
    public class DocumentServiceManager
    {
        public static DocumentServiceManager Instance { get; private set; }

        static DocumentServiceManager()
        {
            DocumentServiceManager.Instance = new DocumentServiceManager();
            DocumentServiceManager.Instance.Register(InternalDocumentService.Instance);
        }



        private static string NormalizeScheme(string scheme)
        {
            return scheme.Trim().ToLower();
        }

        private readonly Dictionary<string, IDocumentService> _services;

        public IEnumerable<IDocumentService> Services { get { return _services.Values; } }

        private DocumentServiceManager()
        {
            _services = new Dictionary<string, IDocumentService>();
        }

        public void Register(IDocumentService service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            if (service.SupportedSchemes == null || service.SupportedSchemes.Length == 0)
                throw new ArgumentException("service", "one or more supported schemes required");

            foreach (var scheme in service.SupportedSchemes)
            {
                var normalizedScheme = DocumentServiceManager.NormalizeScheme(scheme);
                this.LogInfo("registering DocumentService for scheme '{0}'", normalizedScheme);

                IDocumentService existedService;
                if (_services.TryGetValue(normalizedScheme, out existedService))
                    this.LogWarning("a DocumentService of file type '{0}' is already existed, it will be replaced",
                                    normalizedScheme);

                _services[normalizedScheme] = service;
            }
        }


        public IDocumentService GetDocumentService(string scheme)
        {
            scheme = DocumentServiceManager.NormalizeScheme(scheme);
            IDocumentService service;
            if (_services.TryGetValue(scheme, out service))
                return service;

            return null;
        }

        public IDocumentService GetDocumentServiceOrNotifyMissing(string scheme)
        {
            var service = this.GetDocumentService(scheme);
            if (service == null)
                DialogManager.Instance.ShowMessageAsync(this.L("document", "missing_document_service_module_message_title"),
                                                        this.L("document", "missing_document_service_module_message", scheme));

            return service;
        }


    }
}

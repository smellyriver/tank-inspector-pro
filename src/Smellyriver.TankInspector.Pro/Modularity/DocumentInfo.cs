using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Smellyriver.TankInspector.Pro.Modularity.Features;

namespace Smellyriver.TankInspector.Pro.Modularity
{
    public class DocumentInfo : LayoutContentInfo
    {
        public static DocumentInfo CreateTemporary(string repositoryId, string title, Uri uri, FrameworkElement content, IFeature[] features = null)
        {
            return new DocumentInfo(Guid.NewGuid(), true, repositoryId, uri, title, content, features, null);
        }

        public bool IsTemporary { get; private set; }
        public Uri Uri { get; private set; }

        private readonly IFeature[] _features;
        public IEnumerable<IFeature> Features { get { return _features; } }
        public string RepositoryId { get; set; }
        public IDocumentPersistentInfoProvider PersistentInfoProvider { get; set; }

        private DocumentInfo(Guid guid,
                             bool isTemporary,
                             string repositoryId,
                             Uri uri,
                             string title,
                             FrameworkElement content,
                             IFeature[] features,
                             IDocumentPersistentInfoProvider persistentInfoProvider)
            : base(guid)
        {

            _features = features == null ? new IFeature[0] : features;

            this.IsTemporary = isTemporary;
            this.Uri = uri;
            this.Title = title;
            this.Content = content;
            this.RepositoryId = repositoryId;
            this.PersistentInfoProvider = persistentInfoProvider;
        }

        public DocumentInfo(Guid guid,
                            string repositoryId,
                            Uri uri,
                            string title,
                            FrameworkElement content,
                            IFeature[] features = null,
                            IDocumentPersistentInfoProvider persistentInfoProvider = null)
            : this(guid, false, repositoryId, uri, title, content, features, persistentInfoProvider)
        {
        }

        public TFeature GetFeature<TFeature>()
            where TFeature : IFeature
        {
            return _features.OfType<TFeature>().FirstOrDefault();
        }

        public IFeature GetFeature(Type featureType)
        {
            return _features.FirstOrDefault(f => featureType.IsAssignableFrom(f.GetType()));
        }

    }
}

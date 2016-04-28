using System;
using System.Runtime.Serialization;

namespace Smellyriver.TankInspector.Pro.Modularity
{
    [DataContract]
    struct DocumentRestorationInfo
    {
        [DataMember]
        public Guid Guid { get; set; }

        [DataMember]
        public Uri Uri { get; set; }

        [DataMember]
        public string PersistentInfo { get; set; }

        [DataMember]
        public string Title { get; set; }

        public DocumentRestorationInfo(DocumentInfo document)
            : this()
        {
            this.Guid = document.Guid;
            this.Uri = document.Uri;
            this.PersistentInfo = document.PersistentInfoProvider == null ? null : document.PersistentInfoProvider.PersistentInfo;
            this.Title = document.Title;
        }
    }
}

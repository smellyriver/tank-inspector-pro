using System;
using System.Runtime.Serialization;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.Data.Tank
{
    [DataContract]
    public struct NationUnikey
    {
        [DataMember]
        public string RepositoryID { get; set; }
        [DataMember]
        public string NationKey { get; set; }

        public NationUnikey(string repositoryID, string nationKey)
            : this()
        {
            this.RepositoryID = repositoryID;
            this.NationKey = nationKey;
        }

        public NationUnikey(IRepository repository, string nationKey)
            : this(repository.ID, nationKey)
        {

        }

        public Uri CreateUri(string scheme)
        {
            return new Uri(string.Format("{0}://{1}", scheme, this.ToString()), UriKind.Absolute);
        }

        public override string ToString()
        {
            return string.Format("{0};{1}",
                                 this.RepositoryID,
                                 Uri.EscapeDataString(this.NationKey));
        }

        public override bool Equals(object obj)
        {
            if (obj is NationUnikey)
            {
                var other = (NationUnikey)obj;
                return this.RepositoryID == other.RepositoryID
                    && this.NationKey == other.NationKey;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

    }
}

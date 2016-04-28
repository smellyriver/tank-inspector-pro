using System;
using System.Runtime.Serialization;

using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.Data.Tank
{
    [DataContract]
    public struct TankUnikey
    {
        [DataMember]
        public string RepositoryID { get; set; }
        [DataMember]
        public string NationKey { get; set; }
        [DataMember]
        public string TankKey { get; set; }

        public TankUnikey(string repositoryID, string nationKey, string tankKey)
            : this()
        {
            this.RepositoryID = repositoryID;
            this.NationKey = nationKey;
            this.TankKey = tankKey;
        }

        public TankUnikey(IRepository repository, IXQueryable tank)
            : this(repository.ID, tank["nation/@key"], tank["@key"])
        {

        }

        public Uri CreateUri(string scheme)
        {
            return new Uri(string.Format("{0}://{1}", scheme, this.ToString()), UriKind.Absolute);
        }

        public IXQueryable GetTank(IRepository repository)
        {
            return repository.TankDatabase.Query("tank[@key = '{0}' and nation/@key = '{1}']", this.TankKey, this.NationKey);
        }

        public IXQueryable GetTank()
        {
            return this.GetTank(Core.Support.GetRepository(this.RepositoryID));
        }

        public bool TryGetTank(out IXQueryable tank, out IRepository repository)
        {
            repository = Core.Support.GetRepository(this.RepositoryID);
            if(repository == null)
            {
                tank = null;
                return false;
            }

            tank = this.GetTank(repository);
            return tank != null;
        }

        public override string ToString()
        {
            return string.Format("{0};{1}:{2}",
                                 this.RepositoryID,
                                 Uri.EscapeDataString(this.NationKey),
                                 Uri.EscapeDataString(this.TankKey));
        }

        public override bool Equals(object obj)
        {
            if(obj is TankUnikey)
            {
                var other = (TankUnikey)obj;
                return this.RepositoryID == other.RepositoryID
                    && this.TankKey == other.TankKey
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

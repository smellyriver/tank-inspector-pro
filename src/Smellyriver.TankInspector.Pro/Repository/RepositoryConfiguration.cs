using System.IO;
using System.Runtime.Serialization;
using System.Windows.Media;
using Smellyriver.TankInspector.Common;

namespace Smellyriver.TankInspector.Pro.Repository
{
    [DataContract(Namespace = RepositoryHelper.Xmlns)]
    public class RepositoryConfiguration : NotificationObject
    {
        private static readonly DataContractSerializer s_serializer;

        static RepositoryConfiguration()
        {
            s_serializer = new DataContractSerializer(typeof(RepositoryConfiguration));
        }

        public static RepositoryConfiguration Load(string path)
        {
            using (var file = File.OpenRead(path))
                return (RepositoryConfiguration)s_serializer.ReadObject(file);
        }

        public static void Save(RepositoryConfiguration config, string path)
        {
            using (var file = File.Create(path))
                s_serializer.WriteObject(file, config);
        }

        [DataMember(Name = "Alias")]
        private string _alias;
        public string Alias
        {
            get { return _alias; }
            internal set
            {
                _alias = value;
                this.RaisePropertyChanged(() => this.Alias);
            }
        }


        [DataMember(Name = "Language")]
        private string _language;

        public string Language
        {
            get { return _language; }
            set
            {
                _language = value;
                this.RaisePropertyChanged(() => this.Language);
            }
        }


        [DataMember(Name = "MarkerColor")]
        private Color _markerColor;
        public Color MarkerColor
        {
            get { return _markerColor; }
            internal set
            {
                _markerColor = value;
                this.RaisePropertyChanged(() => this.MarkerColor);
            }
        }


        public RepositoryConfiguration(IRepository repository)
        {
            this.Alias = repository.Name;
            this.Language = repository.Language;
        }
    }
}

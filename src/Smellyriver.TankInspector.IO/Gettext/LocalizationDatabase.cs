using System.Collections.Generic;
using System.IO;

namespace Smellyriver.TankInspector.IO.Gettext
{
    public class LocalizationDatabase
    {
        private readonly string _textFolder;

        public string TextFolder
        {
            get { return _textFolder; }
        }

        private readonly Dictionary<string, Catalog> _moDatum = new Dictionary<string, Catalog>();

        private readonly object _catalogReadLock = new object();

        public LocalizationDatabase(string textFolder)
        {
            _textFolder = textFolder;
        }

        public string GetText(string key)
        {
            if (string.IsNullOrEmpty(key))
                return "";

            var info = TextKey.Parse(key);

            if (string.IsNullOrEmpty(info.BaseName))
                return info.MessageID;

            var database = GetData(info.BaseName);

            if (database == null)
                return string.Format("$missing: {0}", key);

            return database.Gettext(info.MessageID);
        }

        private Catalog GetData(string baseName)
        {
            Catalog database;
            lock (_catalogReadLock)
            {
                if (!_moDatum.TryGetValue(baseName, out database))
                {
                    var path = Path.Combine(this.TextFolder, baseName + ".mo");
                    if (!File.Exists(path))
                        return null;

                    database = Catalog.ReadFrom(path);
                    _moDatum.Add(baseName, database);
                }
            }
            return database;
        }

    }
}

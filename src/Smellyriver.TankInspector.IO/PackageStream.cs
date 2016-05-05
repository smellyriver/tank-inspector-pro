using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
#if !MOBILE
using log4net;
#endif

namespace Smellyriver.TankInspector.IO
{
    public class PackageStream : Stream
    {

#if !MOBILE
        // ReSharper disable once InconsistentNaming
        private static readonly ILog log = SafeLog.GetLogger("PackageStream");
#endif

        private static readonly Dictionary<string, ZipFile> s_cachedZipFiles;
        private static readonly object s_cachedZipFilesSyncObject = new object();

        static PackageStream()
        {
            ZipConstants.DefaultCodePage = Encoding.UTF8.CodePage;
            s_cachedZipFiles = new Dictionary<string, ZipFile>();
        }

        private Stream _zipStream;

        private static Exception FileNotFoundException(string path, string packageFile)
        {
            return new FileNotFoundException(string.Format("cannot find {0} in {1}", path, packageFile));
        }

        public static ZipFile GetZipFile(string packageFile)
        {
            if (!File.Exists(packageFile))
                return null;

            packageFile = PackageStream.NormalizePath(packageFile);

            ZipFile zipFile;

            if (!s_cachedZipFiles.TryGetValue(packageFile, out zipFile))
            {
                var stream = File.OpenRead(packageFile);

                zipFile = new ZipFile(stream);

                lock (s_cachedZipFilesSyncObject)
                    s_cachedZipFiles[packageFile] = zipFile;

            }

            return zipFile;
        }


        public static long? GetCrc(string packageFile, string path)
        {
#if !MOBILE
            log.InfoFormat("open package file {0} from {1}", path, packageFile);
#endif
            var zipFile = PackageStream.GetZipFile(PackageStream.NormalizePath(packageFile));
            try
            {
                var entryIndex = zipFile.FindEntry(path, true);
                var entry = zipFile[entryIndex];
                return entry.HasCrc ? entry.Crc : (long?)null;
            }
            catch (Exception)
            {
                throw PackageStream.FileNotFoundException(path, packageFile);
            }
        }

        public static bool IsFileExisted(string packageFile, string path)
        {
            if (!File.Exists(packageFile))
                return false;

            var zipFile = PackageStream.GetZipFile(packageFile);
            return zipFile.FindEntry(path, true) != -1;
        }

        public static bool IsFileExisted(IPackageIndexer indexer, string path)
        {
            if (indexer == null)
                return false;
            return indexer.GetPackagePath(path) != null;
        }

        public static string[] GetFileEntries(string packageFile)
        {
            if (!File.Exists(packageFile))
                return null;

            var zipFile = PackageStream.GetZipFile(packageFile);
            return zipFile.Cast<ZipEntry>().Select(e => e.Name).ToArray();
        }

        public static string NormalizePath(string path)
        {
            if (path == null)
                return null;

            return path.Replace('\\', '/').ToLower();
        }

        public PackageStream(IPackageIndexer indexer, string path)
            : this(indexer.GetPackagePath(path), path)
        {

        }

        public PackageStream(string packageFile, string path)
        {
#if !MOBILE
            log.InfoFormat("open package file {0} from {1}", path, packageFile);
#endif
            var zipFile = PackageStream.GetZipFile(PackageStream.NormalizePath(packageFile));

            if (zipFile == null)
                throw PackageStream.FileNotFoundException(path, packageFile);

            var entryIndex = zipFile.FindEntry(path, true);
            if (entryIndex == -1)
                throw PackageStream.FileNotFoundException(path, packageFile);

            try
            {
                _zipStream = zipFile.GetInputStream(entryIndex);
            }
            catch (Exception ex)
            {
                throw new IOException(string.Format("failed to open {0} in {1}", path, packageFile), ex);
            }
        }


        public override bool CanRead
        {
            get { return _zipStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _zipStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _zipStream.CanWrite; }
        }

        public override void Flush()
        {
            _zipStream.Flush();
        }

        public override long Length
        {
            get { return _zipStream.Length; }
        }

        public override long Position
        {
            get { return _zipStream.Position; }
            set { _zipStream.Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _zipStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _zipStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _zipStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _zipStream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (_zipStream != null)
                {
                    _zipStream.Dispose();
                    _zipStream = null;
                }
            }
        }

    }
}

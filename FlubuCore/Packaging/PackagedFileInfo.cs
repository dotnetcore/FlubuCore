using FlubuCore.IO;

namespace FlubuCore.Packaging
{
    public class PackagedFileInfo
    {
        private LocalPath _localPath;

        private FileFullPath _fileFullPath;

        public PackagedFileInfo(FileFullPath fileFullPath, LocalPath localPath)
        {
            _localPath = localPath;
            _fileFullPath = fileFullPath;
        }

        public PackagedFileInfo(string fullPath, string localPath)
            : this(new FileFullPath(fullPath), new LocalPath(localPath))
        {
        }

        public PackagedFileInfo(FileFullPath fileFullPath)
        {
            _fileFullPath = fileFullPath;
        }

        public LocalPath LocalPath
        {
            get { return _localPath; }
        }

        public FileFullPath FileFullPath
        {
            get { return _fileFullPath; }
        }

        public static PackagedFileInfo FromLocalPath(string path)
        {
            return new PackagedFileInfo(new FileFullPath(path));
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            PackagedFileInfo that = (PackagedFileInfo)obj;

            return string.Equals(_localPath, that._localPath) && string.Equals(_fileFullPath, that._fileFullPath);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        ////private void AssertIsFullPath(string path)
        ////{
        ////    if (false == Path.IsPathRooted(path))
        ////    {
        ////        string message = string.Format(
        ////            CultureInfo.InvariantCulture,
        ////            "Path '{0}' must be absolute.",
        ////            path);
        ////        throw new ArgumentException("path", message);
        ////    }
        ////}
    }
}
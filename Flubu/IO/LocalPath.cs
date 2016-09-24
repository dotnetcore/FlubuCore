using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Flubu.IO
{
    public class LocalPath : IPathBuilder
    {
        private readonly string _localPath;

        public LocalPath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                throw new ArgumentException("Path must be local", "path");
            }

            _localPath = path;
        }

        public string FileName
        {
            get { return Path.GetFileName(_localPath); }
        }

        public LocalPath Flatten
        {
            get { return new LocalPath(FileName); }
        }

        public int Length
        {
            get { return _localPath.Length; }
        }

        /// <summary>
        /// Gets the path that is a parent to the current path in this object.
        /// </summary>
        /// <value>The parent path.</value>
        public LocalPath ParentPath
        {
            get { return new LocalPath(Path.GetDirectoryName(_localPath)); }
        }

        public static implicit operator string(LocalPath path)
        {
            return path.ToString();
        }

        public LocalPath CombineWith(LocalPath path)
        {
            return new LocalPath(Path.Combine(_localPath, path.ToString()));
        }

        public LocalPath CombineWith(string path)
        {
            return CombineWith(new LocalPath(path));
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            LocalPath that = (LocalPath)obj;
            return string.Equals(_localPath, that._localPath);
        }

        public override int GetHashCode()
        {
            return _localPath.GetHashCode();
        }

        public override string ToString()
        {
            return _localPath;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context;
using GlobExpressions;

namespace FlubuCore.IO
{
    public class FullPath : IPathBuilder
    {
        private readonly string _fullPath;

        public FullPath(string path)
        {
            _fullPath = Path.IsPathRooted(path) ? path : Path.GetFullPath(path);
        }

        public bool DirectoryExists => Directory.Exists(_fullPath);

        public bool EndsWithDirectorySeparator => _fullPath.EndsWith(
                                                      Path.DirectorySeparatorChar.ToString(),
                                                      StringComparison.OrdinalIgnoreCase)
                                                  || _fullPath.EndsWith(
                                                      Path.AltDirectorySeparatorChar.ToString(),
                                                      StringComparison.OrdinalIgnoreCase);

        public string FileName => Path.GetFileName(_fullPath);

        public int Length => _fullPath.Length;

        /// <summary>
        /// Gets the path that is a parent to the current path in this object.
        /// </summary>
        /// <value>The parent path.</value>
        public FullPath ParentPath => new FullPath(Path.GetDirectoryName(_fullPath));

        public static implicit operator string(FullPath path)
        {
            return path.ToString();
        }

        /// <summary>
        /// Gets all files matching glob pattern.
        /// See: https://github.com/kthompson/glob for supported pattern expressions and use cases.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="directory"></param>
        /// <param name="globPattern"></param>
        /// <returns></returns>
        public List<FileFullPath> GetFiles(params string[] globPattern)
        {
            return GetFiles(GlobOptions.None, globPattern);
        }

        /// <summary>
        /// Gets all files matching glob pattern.
        /// See: https://github.com/kthompson/glob for supported pattern expressions and use cases.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="directory"></param>
        /// <param name="globPattern"></param>
        /// <returns></returns>
        public List<FileFullPath> GetFiles(GlobOptions globOptions = GlobOptions.None, params string[] globPattern)
        {
            var directoryInfo = new DirectoryInfo(_fullPath);
            return globPattern.SelectMany(pattern => Glob.Files(directoryInfo, pattern, globOptions)).Select(x => new FileFullPath(x.FullName)).ToList();
        }

        public FileFullPath AddFileName(string fileNameFormat, params object[] args)
        {
            string fileName = string.Format(
                CultureInfo.InvariantCulture,
                fileNameFormat,
                args);
            return new FileFullPath(CombineWith(new LocalPath(fileName)).ToString());
        }

        public FullPath AddRaw(string rawString)
        {
            return new FullPath(_fullPath + rawString);
        }

        public FullPath CombineWith(LocalPath localPath)
        {
            return new FullPath(Path.Combine(_fullPath, localPath.ToString()));
        }

        public FullPath CombineWith(string localPath)
        {
            return CombineWith(new LocalPath(localPath));
        }

        public LocalPath DebasePath(FullPath basePath)
        {
            if (!IsSubpathOf(basePath))
            {
                return null;
            }

            if (basePath.Length > 0 && !basePath.EndsWithDirectorySeparator)
            {
                basePath = basePath.AddRaw(Path.DirectorySeparatorChar.ToString());
            }

            string debased = _fullPath.Substring(basePath.Length);
            if (debased.StartsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.OrdinalIgnoreCase)
                || debased.StartsWith(Path.AltDirectorySeparatorChar.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                debased = debased.Substring(1);
            }

            return new LocalPath(debased);
        }

        public void EnsureExists()
        {
            if (!Directory.Exists(_fullPath))
            {
                Directory.CreateDirectory(_fullPath);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            FullPath that = (FullPath)obj;
            return string.Equals(_fullPath, that._fullPath);
        }

        public override int GetHashCode()
        {
            return _fullPath.GetHashCode();
        }

        public bool IsSubpathOf(FullPath basePath)
        {
            return _fullPath.StartsWith(basePath.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return _fullPath;
        }

        public FileFullPath ToFileFullPath()
        {
            return new FileFullPath(_fullPath);
        }
    }
}

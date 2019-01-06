using System.IO;
using FlubuCore.Context;

namespace FlubuCore.Tasks.FileSystem
{
    public class DeleteFilesTask : TaskBase<int, DeleteFilesTask>
    {
        private string _directoryPath;
        private string _filePattern;
        private bool _recursive;
        private string _description;

        /// <summary>
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="filePattern">The search string to match against the names of files in path. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters but doesnt support regular expressions.</param>
        /// <param name="recursive">If true it searches and deletes all matching files in subdirectories.</param>
        public DeleteFilesTask(string directoryPath, string filePattern, bool recursive)
        {
            _directoryPath = directoryPath;
            _filePattern = filePattern;
            _recursive = recursive;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Deletes all files that matches '{_filePattern}' pattern in folder '{_directoryPath}' ";
                }

                return _description;
            }

            set { _description = value; }
        }

        public static void Execute(
            ITaskContextInternal context,
            string directoryPath,
            string filePattern,
            bool recursive)
        {
            var task = new DeleteFilesTask(directoryPath, filePattern, recursive);
            task.ExecuteVoid(context);
        }

        public DeleteFilesTask DirectoryPath(string directoryPath)
        {
            _directoryPath = directoryPath;
            return this;
        }

        /// <summary>
        /// The search string to match against the names of files in path. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters but doesnt support regular expressions.
        /// </summary>
        /// <param name="filePattern"></param>
        /// <returns></returns>
        public DeleteFilesTask FilePattern(string filePattern)
        {
            _filePattern = filePattern;
            return this;
        }

        /// <summary>
        /// If true it searches and deletes all matching files in subdirectories.
        /// </summary>
        /// <returns></returns>
        public DeleteFilesTask Recursive()
        {
            _recursive = true;
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            DoLogInfo($"Delete files from directory {_directoryPath} matching pattern '{_filePattern}'");

            var searchOption = SearchOption.TopDirectoryOnly;

            if (_recursive)
                searchOption = SearchOption.AllDirectories;

            foreach (var file in Directory.EnumerateFiles(_directoryPath, _filePattern, searchOption))
            {
                File.Delete(file);
                DoLogInfo($"Deleted file '{file}'");
            }

            return 0;
        }
    }
}
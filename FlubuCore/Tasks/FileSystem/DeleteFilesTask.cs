using System.IO;
using FlubuCore.Context;

namespace FlubuCore.Tasks.FileSystem
{
    public class DeleteFilesTask : TaskBase<int, DeleteFilesTask>
    {
        private readonly string _directoryPath;
        private readonly string _filePattern;
        private readonly bool _recursive;
        private string _description;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="filePattern">The search string to match against the names of files in path. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters but doesnt support regular expressions.</param>
        /// <param name="recursive"></param>
        public DeleteFilesTask(string directoryPath, string filePattern, bool recursive)
        {
            _directoryPath = directoryPath;
            _filePattern = filePattern;
            _recursive = recursive;
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

        protected override int DoExecute(ITaskContextInternal context)
        {
            context.LogInfo($"Delete files from directory {_directoryPath} matching pattern '{_filePattern}'");

            var searchOption = SearchOption.TopDirectoryOnly;

            if (_recursive)
                searchOption = SearchOption.AllDirectories;

            foreach (var file in Directory.EnumerateFiles(_directoryPath, _filePattern, searchOption))
            {
                File.Delete(file);
                context.LogInfo($"Deleted file '{file}'");
            }

            return 0;
        }
    }
}
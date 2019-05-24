using System.IO;
using FlubuCore.Context;

namespace FlubuCore.Tasks.FileSystem
{
    /// <summary>
    /// Task creates directroy
    /// </summary>
    public class CreateDirectoryTask : TaskBase<int, CreateDirectoryTask>
    {
        private bool _forceRecreate;
        private string _description;

        /// <summary>
        /// Task creates directory at the given location.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="forceRecreate">If <c>true</c> directory is deleted if it exists and then created again.</param>
        public CreateDirectoryTask(string directoryPath, bool forceRecreate)
        {
            DirectoryPath = directoryPath;
            _forceRecreate = forceRecreate;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Creates directory {DirectoryPath}";
                }

                return _description;
            }

            set { _description = value; }
        }

        protected string DirectoryPath { get; set; }

        public static void Execute(ITaskContextInternal context, string directoryPath, bool forceRecreate)
        {
            var task = new CreateDirectoryTask(directoryPath, forceRecreate);
            task.ExecuteVoid(context);
        }

        public CreateDirectoryTask SetDirectoryPath(string directoryPath)
        {
            DirectoryPath = directoryPath;
            return this;
        }

        public CreateDirectoryTask ForceRecreate()
        {
            _forceRecreate = true;
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            DoLogInfo($"Create directory '{DirectoryPath}'");

            if (Directory.Exists(DirectoryPath))
            {
                if (!_forceRecreate)
                    return 0;

                Directory.Delete(DirectoryPath, true);
            }

            Directory.CreateDirectory(DirectoryPath);

            return 0;
        }
    }
}
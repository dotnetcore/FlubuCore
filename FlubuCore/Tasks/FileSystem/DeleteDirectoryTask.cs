using System.IO;
using FlubuCore.Context;

namespace FlubuCore.Tasks.FileSystem
{
    public class DeleteDirectoryTask : TaskBase
    {
        private readonly string _directoryPath;
        private readonly bool _failIfNotExists;

        public DeleteDirectoryTask(string directoryPath, bool failIfNotExists)
        {
            _directoryPath = directoryPath;
            _failIfNotExists = failIfNotExists;
        }

        public static void Execute(
            ITaskContext context,
            string directoryPath,
            bool failIfNotExists)
        {
            var task = new DeleteDirectoryTask(directoryPath, failIfNotExists);
            task.Execute(context);
        }

        protected override int DoExecute(ITaskContext context)
        {
            context.LogInfo($"Delete directory '{_directoryPath}'");

            if (!Directory.Exists(_directoryPath))
            {
                if (!_failIfNotExists)
                    return 0;
            }

            Directory.Delete(_directoryPath, true);

            return 0;
        }
    }
}
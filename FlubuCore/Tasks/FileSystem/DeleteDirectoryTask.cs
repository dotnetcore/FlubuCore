using System;
using System.IO;
using FlubuCore.Context;

namespace FlubuCore.Tasks.FileSystem
{
    public class DeleteDirectoryTask : TaskBase<int>
    {
        private readonly string _directoryPath;

        private readonly bool _failIfNotExists;

        public DeleteDirectoryTask(string directoryPath, bool failIfNotExists)
        {
            _directoryPath = directoryPath;
            _failIfNotExists = failIfNotExists;
        }

        public static void Execute(
            ITaskContextInternal context,
            string directoryPath,
            bool failIfNotExists)
        {
            var task = new DeleteDirectoryTask(directoryPath, failIfNotExists);
            task.ExecuteVoid(context);
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            context.LogInfo($"Delete directory '{_directoryPath}'");

            if (!Directory.Exists(_directoryPath))
            {
                if (!_failIfNotExists)
                    return 0;
            }

            try
            {
                Directory.Delete(_directoryPath, true);
            }
            catch (Exception)
            {
                if (!DoNotFail)
                    throw;
            }

            return 0;
        }
    }
}
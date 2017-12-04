using System;
using System.IO;
using FlubuCore.Context;

namespace FlubuCore.Tasks.FileSystem
{
    public class DeleteDirectoryTask : TaskBase<int, DeleteDirectoryTask>
    {
        private readonly string _directoryPath;

        private readonly bool _failIfNotExists;
        private string _description;

        public DeleteDirectoryTask(string directoryPath, bool failIfNotExists)
        {
            _directoryPath = directoryPath;
            _failIfNotExists = failIfNotExists;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Deletes directory '{_directoryPath}'.";
                }

                return _description;
            }

            set { _description = value; }
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
using System.IO;
using FlubuCore.Context;

namespace FlubuCore.Tasks.FileSystem
{
    public class CopyFileTask : TaskBase
    {
        private readonly string _destinationFileName;
        private readonly bool _overwrite;

        private readonly string _sourceFileName;

        public CopyFileTask(
            string sourceFileName,
            string destinationFileName,
            bool overwrite)
        {
            _sourceFileName = sourceFileName;
            _destinationFileName = destinationFileName;
            _overwrite = overwrite;
        }

        public static void Execute(
            ITaskContext context,
            string sourceFileName,
            string destinationFileName,
            bool overwrite)
        {
            var task = new CopyFileTask(sourceFileName, destinationFileName, overwrite);
            task.Execute(context);
        }

        protected override int DoExecute(ITaskContext context)
        {
            context.WriteMessage($"Copy file '{_sourceFileName}' to '{_destinationFileName}'");

            var dir = Path.GetDirectoryName(_destinationFileName);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.Copy(_sourceFileName, _destinationFileName, _overwrite);

            return 0;
        }
    }
}
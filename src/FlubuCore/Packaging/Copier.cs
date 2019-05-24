using System.IO;
using FlubuCore.Context;
using FlubuCore.IO;

namespace FlubuCore.Packaging
{
    public class Copier : ICopier
    {
        private readonly bool _logCopiedFiles;
        private readonly ITaskContextInternal _taskContext;

        public Copier(ITaskContextInternal taskContext, bool logCopiedFiles = true)
        {
            _taskContext = taskContext;
            _logCopiedFiles = logCopiedFiles;
        }

        public void Copy(FileFullPath sourceFileName, FileFullPath destinationFileName)
        {
            var directoryName = destinationFileName.Directory.ToString();

            if (!string.IsNullOrEmpty(directoryName))
            {
                if (!Directory.Exists(directoryName))
                {
                    if (_logCopiedFiles)
                    {
                        _taskContext.LogInfo($"Creating directory '{directoryName}'");
                    }

                    Directory.CreateDirectory(directoryName);
                }
            }

            if (_logCopiedFiles)
                    _taskContext.LogInfo($"Copying file '{sourceFileName}' to '{destinationFileName}'");

            File.Copy(sourceFileName.ToString(), destinationFileName.ToString(), true);
        }
    }
}
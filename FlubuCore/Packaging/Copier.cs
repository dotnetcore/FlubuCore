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
                    _taskContext.LogInfo(string.Format("Creating directory '{0}'", directoryName));
                    Directory.CreateDirectory(directoryName);
                }
            }

            if (_logCopiedFiles)
                    _taskContext.LogInfo(string.Format("Copying file '{0}' to '{1}'", sourceFileName, destinationFileName));

            File.Copy(sourceFileName.ToString(), destinationFileName.ToString(), true);
        }
    }
}
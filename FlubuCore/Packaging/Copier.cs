using System;
using System.IO;
using Flubu.Context;
using Flubu.IO;
using Flubu.Tasks;

namespace Flubu.Packaging
{
    public class Copier : ICopier
    {
        private readonly ITaskContext _taskContext;

        public Copier(ITaskContext taskContext)
        {
           _taskContext = taskContext;
        }

        public void Copy(FileFullPath sourceFileName, FileFullPath destinationFileName)
        {
            string directoryName = destinationFileName.Directory.ToString();

            if (!string.IsNullOrEmpty(directoryName))
            {
                if (!Directory.Exists(directoryName))
                {
                    _taskContext.WriteMessage(string.Format("Creating directory '{0}'", directoryName));
                    Directory.CreateDirectory(directoryName);
                }
            }

            _taskContext.WriteMessage(string.Format("Copying file '{0}' to '{1}'", sourceFileName, destinationFileName));
            File.Copy(sourceFileName.ToString(), destinationFileName.ToString(), true);
        }
    }
}
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Packaging
{
    public class UnzipTask : TaskBase<int>
    {
        private readonly string _fileName;
        private readonly string _destination;

        public UnzipTask(string zipFile, string destinationFolder)
        {
            _fileName = zipFile;
            _destination = destinationFolder;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            context.LogInfo($"Extract {_fileName} to {_destination}");

            if (!Directory.Exists(_destination))
                Directory.CreateDirectory(_destination);

            ZipFile.ExtractToDirectory(_fileName, _destination);
            return 0;
        }
    }
}

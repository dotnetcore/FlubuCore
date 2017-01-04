using System.IO;
using System.IO.Compression;
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

            using (Stream zip = File.OpenRead(_fileName))
            using (ZipArchive archive = new ZipArchive(zip, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string file = Path.Combine(_destination, entry.FullName);
                    context.LogInfo($"Extract {entry.FullName} -> {file}");
                    entry.ExtractToFile(file, true);
                }
            }

            return 0;
        }
    }
}

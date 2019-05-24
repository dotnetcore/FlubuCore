using System.IO;
using FlubuCore.Context;

namespace FlubuCore.Tasks.FileSystem
{
    /// <summary>
    ///     Copies file to specified soruce
    /// </summary>
    public class CopyFileTask : TaskBase<int, CopyFileTask>
    {
        private string _destinationFileName;
        private bool _overwrite;

        private string _sourceFileName;
        private string _description;

        /// <summary>
        ///     Copies file to specified destination location.
        /// </summary>
        /// <param name="sourceFileName">Path of file to be copied. </param>
        /// <param name="destinationFileName">Destination location of the file to be copied.</param>
        /// <param name="overwrite">if <c>true</c> file on the destionation location is overwriren if it exists. Otherwise not.</param>
        public CopyFileTask(
            string sourceFileName,
            string destinationFileName,
            bool overwrite)
        {
            _sourceFileName = sourceFileName;
            _destinationFileName = destinationFileName;
            _overwrite = overwrite;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                    return $"Copies file '{_sourceFileName}' to '{_destinationFileName}'.";

                return _description;
            }

            set => _description = value;
        }

        /// <summary>
        /// The source path.
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <returns></returns>
        public CopyFileTask SourcePath(string sourceFileName)
        {
            _sourceFileName = sourceFileName;
            return this;
        }

        /// <summary>
        /// The destination path.
        /// </summary>
        /// <param name="destinationFileName"></param>
        /// <returns></returns>
        public CopyFileTask DestinationPath(string destinationFileName)
        {
            _destinationFileName = destinationFileName;
            return this;
        }

        public CopyFileTask OverwriteExisting()
        {
            _overwrite = true;
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            DoLogInfo($"Copy file '{_sourceFileName}' to '{_destinationFileName}'");

            var dir = Path.GetDirectoryName(_destinationFileName);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.Copy(_sourceFileName, _destinationFileName, _overwrite);

            return 0;
        }
    }
}
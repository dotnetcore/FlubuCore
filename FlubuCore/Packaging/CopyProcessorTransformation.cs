using FlubuCore.IO;

namespace FlubuCore.Packaging
{
    public class CopyProcessorTransformation
    {
        private string _sourceId;

        private LocalPath _destinationPath;

        private CopyProcessorTransformationOptions _options = CopyProcessorTransformationOptions.None;

        public CopyProcessorTransformation(
            string sourceId, LocalPath destinationPath, CopyProcessorTransformationOptions options)
        {
            _sourceId = sourceId;
            _destinationPath = destinationPath;
            _options = options;
        }

        public string SourceId
        {
            get { return _sourceId; }
        }

        public LocalPath DestinationPath
        {
            get { return _destinationPath; }
        }

        public CopyProcessorTransformationOptions Options
        {
            get { return _options; }
        }
    }
}
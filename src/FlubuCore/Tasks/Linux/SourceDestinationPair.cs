namespace FlubuCore.Tasks.Linux
{
    internal class SourceDestinationPair
    {
        public SourceDestinationPair(string source, string destination, bool isFile)
        {
            Source = source;
            Destination = destination;
            IsFile = isFile;
        }

        public string Source { get; }

        public string Destination { get; }

        public bool IsFile { get; }
    }
}

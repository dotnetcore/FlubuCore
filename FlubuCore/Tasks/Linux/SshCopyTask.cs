using FlubuCore.Context;
using Renci.SshNet;
using System.Collections.Generic;
using System.IO;

namespace FlubuCore.Tasks.Linux
{
    public class SshCopyTask : TaskBase<int>
    {
        private readonly string _host;
        private readonly string _userName;
        private readonly string _password;
        private string _destination;
        private readonly List<SourceDestinationPair> _items = new List<SourceDestinationPair>();

        private string _sourceDir;

        public SshCopyTask(string host, string userName)
        {
            _host = host;
            _userName = userName;
        }

        public SshCopyTask(string host, string userName, string password)
        {
            _host = host;
            _userName = userName;
            _password = password;
        }

        public SshCopyTask WithFile(string sourceFile, string destinationFile)
        {
            _items.Add(new SourceDestinationPair(sourceFile, destinationFile, true));
            return this;
        }

        public SshCopyTask WithFolder(string sourceDir, string destinationFile)
        {
            _items.Add(new SourceDestinationPair(sourceDir, destinationFile, false));
            return this;
        }


        protected override int DoExecute(ITaskContextInternal context)
        {
            context.LogInfo($"Connecting to {_userName}@{_host}");
            string password = _password.GetPassword();

            using (ScpClient cl = new ScpClient(_host, _userName, password))
            {
                cl.Connect();
                foreach(SourceDestinationPair item in _items)
                {
                    context.LogInfo($"copy {item.Source}->{item.Destination}");

                    if(item.IsFile)
                    {
                        cl.Upload(new FileInfo(item.Source), item.Destination);
                    }
                    else
                    {
                        cl.Upload(new DirectoryInfo(item.Source), item.Destination);

                    }
                }
                cl.Disconnect();
                return 0;
            }
        }
    }
}

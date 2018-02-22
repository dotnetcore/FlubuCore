using System.Collections.Generic;
using System.IO;
using FlubuCore.Context;
using Renci.SshNet;

namespace FlubuCore.Tasks.Linux
{
    public class SshCopyTask : TaskBase<int, SshCopyTask>
    {
        private readonly string _host;
        private readonly List<SourceDestinationPair> _items = new List<SourceDestinationPair>();
        private readonly string _userName;
        private string _password;

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

        protected override string Description { get; set; }

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

        public SshCopyTask WithPassword(string password)
        {
            _password = password;
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            DoLogInfo($"Connecting to {_userName}@{_host}");
            string password = _password.GetPassword();

            using (ScpClient cl = new ScpClient(_host, _userName, password))
            {
                cl.Connect();
                foreach (SourceDestinationPair item in _items)
                {
                    DoLogInfo($"copy {item.Source}->{item.Destination}");

                    if (item.IsFile)
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
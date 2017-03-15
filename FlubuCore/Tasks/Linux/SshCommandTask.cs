using FlubuCore.Context;
using Renci.SshNet;
using System.Collections.Generic;

namespace FlubuCore.Tasks.Linux
{
    public class SshCommandTask : TaskBase<int>
    {
        private readonly string _host;
        private readonly string _userName;
        private readonly string _password;
        private readonly List<string> _commands = new List<string>();

        public SshCommandTask(string host, string userName, string password)
        {
            _host = host;
            _userName = userName;
            _password = password;
        }

        public SshCommandTask(string host, string userName)
        {
            _host = host;
            _userName = userName;
        }

        public SshCommandTask WithCommand(string command)
        {
            _commands.Add(command);
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            context.LogInfo($"Connecting to host {_userName}@{_host}");

            string password = _password.GetPassword();

            using (SshClient client = new SshClient(_host, _userName, password))
            {
                client.Connect();

                foreach (string command in _commands)
                {
                    context.LogInfo($"Executing command {command}");
                    SshCommand cmd = client.CreateCommand(command);
                    string res = cmd.Execute();
                    context.LogInfo($"Command response {res}");
                }
                client.Disconnect();
                return 0;
            }
        }
    }
}

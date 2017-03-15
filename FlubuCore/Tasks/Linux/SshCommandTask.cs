using FlubuCore.Context;
using Renci.SshNet;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Tasks.Linux
{
    public class SshCommandTask : TaskBase<int>
    {
        private readonly string _host;
        private readonly string _userName;
        private string _password;
        private readonly List<string> _commands = new List<string>();

        public SshCommandTask(string host, string userName)
        {
            _host = host;
            _userName = userName;
        }

        public SshCommandTask(string host, string userName, string password)
        {
            _host = host;
            _userName = userName;
            _password = password;
        }

        public SshCommandTask WithCommand(string command)
        {
            _commands.Add(command);
            return this;
        }

        public SshCommandTask WithPassword(string password)
        {
            _password = password;
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            context.LogInfo($"Connecting to host {_userName}@{_host}");

            string password = _password.GetPassword();

            using (SshClient client = new SshClient(_host, _userName, password))
            {
                client.Connect();

                StringBuilder cmdText = new StringBuilder();

                foreach (string command in _commands)
                {
                    context.LogInfo($"Executing command {command}");
                    cmdText.Append($"{command} &&");
                }

                cmdText.Remove(cmdText.Length - 2, 2);

                using (SshCommand cmd = client.CreateCommand(cmdText.ToString()))
                {
                    string res = cmd.Execute();
                    context.LogInfo($"Command response [{res}]");
                }

                client.Disconnect();
                return 0;
            }
        }
    }
}

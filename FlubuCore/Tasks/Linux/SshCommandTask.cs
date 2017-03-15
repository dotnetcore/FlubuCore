using FlubuCore.Context;
using Renci.SshNet;

namespace FlubuCore.Tasks.Linux
{
    public class SshCommandTask : TaskBase<string>
    {
        private readonly string _host;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _command;

        public SshCommandTask(string host, string userName, string password, string command)
        {
            _host = host;
            _userName = userName;
            _password = password;
            _command = command;
        }

        protected override string DoExecute(ITaskContextInternal context)
        {
            context.LogInfo($"Executing command {_command} on {_userName}@{_host}");

            using (SshClient client = new SshClient(_host, _userName, _password))
            {
                client.Connect();

                SshCommand command = client.CreateCommand(_command);
                string res = command.Execute();
                client.Disconnect();
                return res;
            }
        }
    }
}

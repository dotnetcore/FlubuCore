using System.Collections.Generic;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.WebApi.Client;
using FlubuCore.WebApi.Model;

namespace FlubuCore.Tasks.FlubuWebApi
{
    public class ExecuteFlubuScriptTask : WebApiBaseTask<ExecuteFlubuScriptTask, int>
    {
        private readonly string _mainCommand;
        private readonly string _scriptFilePath;
        private readonly List<string> _commands;
        private string _description;

        public ExecuteFlubuScriptTask(string mainCommand, string scriptFilePath, IWebApiClientFactory webApiClient)
            : base(webApiClient)
        {
            _commands = new List<string>();
            _mainCommand = mainCommand;
            _scriptFilePath = scriptFilePath;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Execute flubu script '{_scriptFilePath}' with command '{_mainCommand}'.";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// Adds remaining flubu commands.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public ExecuteFlubuScriptTask AddCommands(params string[] command)
        {
            _commands.AddRange(command);
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            Task<int> task = DoExecuteAsync(context);

            return task.GetAwaiter().GetResult();
        }

        protected override async Task<int> DoExecuteAsync(ITaskContextInternal context)
        {
            var client = WebApiClientFactory.Create(context.Properties.Get<string>(BuildProps.LastWebApiBaseUrl));
            await client.ExecuteScriptAsync(new ExecuteScriptRequest
            {
                ScriptFileName = _scriptFilePath,
                TargetToExecute = _mainCommand,
                RemainingCommands = _commands,
            });
            return 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.WebApi.Client;
using FlubuCore.WebApi.Model;

namespace FlubuCore.Tasks.FlubuWebApi
{
    public class ExecuteFlubuScriptTask : WebApiBaseTask<ExecuteFlubuScriptTask, int>
    {
        private string _mainCommand;

        private string _scriptFilePath;

        private List<string> _commands;
        private string _description;

        public ExecuteFlubuScriptTask(string mainCommand, string scriptFilePath, IWebApiClient webApiClient)
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
                    return $"Execute flubu script '{_scriptFilePath}' with command '{_mainCommand}' on flubu server '{WebApiClient.WebApiBaseUrl}'";
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
            PrepareWebApiClient(context);
            await WebApiClient.ExecuteScriptAsync(new ExecuteScriptRequest
            {
                ScriptFileName = _scriptFilePath,
                TargetToExecute = _mainCommand,
                RemainingCommands = _commands

            });
            return 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.WebApi.Client;
using FlubuCore.WebApi.Model;

namespace FlubuCore.Tasks.FlubuWebApi
{
    public class ExecuteFlubuScriptTask : TaskBase<int>
    {
        private readonly IWebApiClient _webApiClient;

        private string _mainCommand;

        private string _scriptFilePath;

        private List<string> _commands;
        
        public ExecuteFlubuScriptTask(string mainCommand, string scriptFilePath, string webApiBaseUrl, IWebApiClient webApiClient)
        {
            _webApiClient = webApiClient;
            _commands = new List<string>();
            _webApiClient.WebApiBaseUrl = webApiBaseUrl;
            _mainCommand = mainCommand;
            _scriptFilePath = scriptFilePath;
        }

        public ExecuteFlubuScriptTask AddCommands(params string[] command)
        {
            _commands.AddRange(command);
            return this;
        }

        public ExecuteFlubuScriptTask SetTimeout(TimeSpan timeout)
        {
            _webApiClient.Timeout = timeout;
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            Task<int> task = DoExecuteAsync(context);

            return task.GetAwaiter().GetResult();
        }

        protected override async Task<int> DoExecuteAsync(ITaskContextInternal context)
        {
            await _webApiClient.ExecuteScriptAsync(new ExecuteScriptRequest
            {
               ScriptFilePathLocation = _scriptFilePath,
               TargetToExecute = _mainCommand,
               RemainingCommands = _commands
              
            });
            return 0;
        }
    }
}

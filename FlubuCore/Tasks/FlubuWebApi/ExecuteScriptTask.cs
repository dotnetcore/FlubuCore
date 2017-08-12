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

	    private bool _webApiUrlSet = false;

        private List<string> _commands;
        
        public ExecuteFlubuScriptTask(string mainCommand, string scriptFilePath, IWebApiClient webApiClient)
        {
            _webApiClient = webApiClient;
            _commands = new List<string>();
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

	    public ExecuteFlubuScriptTask SetWebApiBaseUrl(string webApiUrl)
	    {
		    _webApiClient.WebApiBaseUrl = webApiUrl;
		    _webApiUrlSet = true;
		    return this;
	    }

		protected override int DoExecute(ITaskContextInternal context)
        {
            Task<int> task = DoExecuteAsync(context);

            return task.GetAwaiter().GetResult();
        }

	    protected override async Task<int> DoExecuteAsync(ITaskContextInternal context)
	    {
		    if (_webApiUrlSet)
		    {
			    _webApiClient.WebApiBaseUrl = context.Properties.GetFlubuWebApiBaseUrl();
		    }

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

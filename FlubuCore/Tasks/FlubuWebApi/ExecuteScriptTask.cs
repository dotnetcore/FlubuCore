using System;
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

        private readonly Dictionary<string, string> _scriptArguments;
        private string _description;

        private ConsoleColor _logsForegroundColor = ConsoleColor.DarkGreen;

        public ExecuteFlubuScriptTask(string mainCommand, string scriptFilePath, IWebApiClientFactory webApiClient)
            : base(webApiClient)
        {
            _scriptArguments = new Dictionary<string, string>();
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
        /// Adds argument with specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ExecuteFlubuScriptTask AddScriptArgument(string key, string value)
        {
            _scriptArguments.Add(key, value);
            return this;
        }

        public ExecuteFlubuScriptTask LogsWithColor(ConsoleColor foregroundColor)
        {
            _logsForegroundColor = foregroundColor;
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
            try
            {
                var response = await client.ExecuteScriptAsync(new ExecuteScriptRequest
                {
                    ScriptFileName = _scriptFilePath,
                    TargetToExecute = _mainCommand,
                    ScriptArguments = _scriptArguments,
                });

                WriteLogs(response.Logs, _logsForegroundColor);
            }
            catch (WebApiException e)
            {
                WriteLogs(e.Logs, _logsForegroundColor);
                throw new TaskExecutionException("Execute script failed!", 99);
            }

            return 0;
        }
    }
}

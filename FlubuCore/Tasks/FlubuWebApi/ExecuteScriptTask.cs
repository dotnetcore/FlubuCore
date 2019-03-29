using System;
using System.Collections.Generic;
#if !NETSTANDARD1_6
using System.Drawing;
#endif
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

#if !NETSTANDARD1_6
        private Color _logsForegroundColor = Color.DarkGreen;
#endif

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

#if !NETSTANDARD1_6
        public ExecuteFlubuScriptTask LogsWithColor(Color foregroundColor)
        {
            _logsForegroundColor = foregroundColor;
            return this;
        }
#endif

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

#if !NETSTANDARD1_6
                WriteLogs(response.Logs, _logsForegroundColor);
#else
                WriteLogs(response.Logs);
#endif
            }
            catch (WebApiException e)
            {
#if !NETSTANDARD1_6
                WriteLogs(e.Logs, _logsForegroundColor);
 #else
                 WriteLogs(e.Logs);
#endif

                throw new TaskExecutionException("Execute script failed!", 99);
            }

            return 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.WebApi.Client;
using FlubuCore.WebApi.Model;

namespace FlubuCore.Tasks.FlubuWebApi
{
    public class UploadScriptTask : WebApiBaseTask<UploadScriptTask, int>
    {
        private readonly string _scriptFilePath;
        private string _description;

        public UploadScriptTask(IWebApiClient webApiClient, string scriptFIlePath)
            : base(webApiClient)
        {
            _scriptFilePath = scriptFIlePath;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Upload flubu script {_scriptFilePath} tp flubu server {WebApiClient.WebApiBaseUrl}";
                }

                return _description;
            }

            set { _description = value; }
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            Task<int> task = DoExecuteAsync(context);

            return task.GetAwaiter().GetResult();
        }

        protected override async Task<int> DoExecuteAsync(ITaskContextInternal context)
        {
            PrepareWebApiClient(context);
            await WebApiClient.UploadScriptAsync(new UploadScriptRequest
            {
                FilePath = _scriptFilePath
            });

            return 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;

using FlubuCore.Context;
using FlubuCore.WebApi.Client;
using FlubuCore.WebApi.Model;

namespace FlubuCore.Tasks.FlubuWebApi
{
    public abstract class WebApiBaseTask<TTask, TReturnT> : TaskBase<TReturnT, TTask>
        where TTask : class, ITask
    {
        protected WebApiBaseTask(IWebApiClientFactory webApiClient)
        {
            WebApiClientFactory = webApiClient;
        }

        protected IWebApiClientFactory WebApiClientFactory { get; set; }

        public void WriteLogs(List<string> logs, Color? foregroundColor)
        {
            if (foregroundColor == null)
            {
                foregroundColor = Color.DarkGreen;
            }

            if (logs == null)
            {
                return;
            }

            foreach (var log in logs)
            {
                DoLogInfo(log, foregroundColor.Value);
            }
        }
    }
}

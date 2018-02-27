using System;
using FlubuCore.Context;
using FlubuCore.WebApi.Client;

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
    }
}

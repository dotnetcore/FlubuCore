using System;
using FlubuCore.Context;
using FlubuCore.WebApi.Client;

namespace FlubuCore.Tasks.FlubuWebApi
{
    public abstract class WebApiBaseTask<TTask, TReturnT> : TaskBase<TReturnT, TTask>
        where TTask : class, ITask
    {
        protected WebApiBaseTask(IWebApiClient webApiClient)
        {
            WebApiClient = webApiClient;
        }

        protected bool WebApiUrlSet { get; set; }

        protected IWebApiClient WebApiClient { get; set; }

        /// <summary>
        /// Set's web api base url on web api client.
        /// </summary>
        /// <param name="webApiUrl"></param>
        /// <returns></returns>
        public TTask SetWebApiBaseUrl(string webApiUrl)
        {
            WebApiClient.WebApiBaseUrl = webApiUrl;
            WebApiUrlSet = true;
            return this as TTask;
        }

        /// <summary>
        /// Set's timeout on web api client.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public TTask SetTimeout(TimeSpan timeout)
        {
            WebApiClient.Timeout = timeout;
            return this as TTask;
        }

        /// <summary>
        /// Set's token on web api client.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public TTask SetToken(string token)
        {
            WebApiClient.Token = token;
            return this as TTask;
        }

        protected void PrepareWebApiClient(ITaskContextInternal context)
        {
            if (!WebApiUrlSet)
            {
                if (string.IsNullOrEmpty(WebApiClient.WebApiBaseUrl))
                {
                    WebApiClient.WebApiBaseUrl = context.Properties.GetFlubuWebApiBaseUrl();
                }
            }
        }
    }
}

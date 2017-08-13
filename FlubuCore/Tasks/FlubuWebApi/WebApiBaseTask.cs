using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.WebApi.Client;

namespace FlubuCore.Tasks.FlubuWebApi
{
    public abstract class WebApiBaseTask<T, ReturnT> : TaskBase<ReturnT> where T : class
    {
	    public WebApiBaseTask(IWebApiClient webApiClient)
	    {
		    this.WebApiClient = webApiClient;

	    }

	    public T SetWebApiBaseUrl(string webApiUrl)
	    {
		    WebApiClient.WebApiBaseUrl = webApiUrl;
		    WebApiUrlSet = true;
		    return this as T;
	    }

	    public T SetTimeout(TimeSpan timeout)
	    {
		    WebApiClient.Timeout = timeout;
			return this as T;
		}

	    public T SetToken(string token)
	    {
		    WebApiClient.Token = token;
		    return this as T;
	    }

		protected void PrepareWebApiClient(ITaskContextInternal context)
	    {
		    if (WebApiUrlSet)
		    {
				WebApiClient.WebApiBaseUrl = context.Properties.GetFlubuWebApiBaseUrl();
			}
		}

		protected bool WebApiUrlSet { get; set; } = false;

		protected IWebApiClient WebApiClient {get; set; }
	}
}

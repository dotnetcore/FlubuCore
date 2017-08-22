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

		/// <summary>
		/// Set's web api base url on web api client.
		/// </summary>
		/// <param name="webApiUrl"></param>
		/// <returns></returns>
	    public T SetWebApiBaseUrl(string webApiUrl)
	    {
		    WebApiClient.WebApiBaseUrl = webApiUrl;
		    WebApiUrlSet = true;
		    return this as T;
	    }

		/// <summary>
		/// Set's timeout on web api client.
		/// </summary>
		/// <param name="timeout"></param>
		/// <returns></returns>
	    public T SetTimeout(TimeSpan timeout)
	    {
		    WebApiClient.Timeout = timeout;
			return this as T;
		}

		/// <summary>
		/// Set's token on web api client.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
	    public T SetToken(string token)
	    {
		    WebApiClient.Token = token;
		    return this as T;
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

		protected bool WebApiUrlSet { get; set; } = false;

		protected IWebApiClient WebApiClient {get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.WebApi.Client;
using FlubuCore.WebApi.Model;

namespace FlubuCore.Tasks.FlubuWebApi
{
    public class UploadPackageTask : TaskBase<int>
    {
	    private readonly IWebApiClient _webApiClient;

	    private string _packageSearchPattern;

	    private string _directoryPath;

	    private bool _webApiUrlSet = false;

		public UploadPackageTask(IWebApiClient client, string directoryPath)
	    {
		    _webApiClient = client;
		    _directoryPath = directoryPath;
	    }

	    public UploadPackageTask PackageSearchPattern(string packageSearchPattern)
	    {
		    _packageSearchPattern = packageSearchPattern;
		    return this;
	    }

	    public UploadPackageTask SetWebApiBaseUrl(string webApiUrl)
	    {
		    _webApiClient.WebApiBaseUrl = webApiUrl;
		    _webApiUrlSet = true;
		    return this;
	    }

		public UploadPackageTask SetTimeout(TimeSpan timeout)
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
		    if (_webApiUrlSet)
		    {
			    _webApiClient.WebApiBaseUrl = context.Properties.GetFlubuWebApiBaseUrl();
		    }

			await _webApiClient.UploadPackageAsync(new UploadPackageRequest
		    {
			    PackageSearchPattern = _packageSearchPattern,
				DirectoryPath = _directoryPath
		    });

		    return 0;
	    }
	}
}

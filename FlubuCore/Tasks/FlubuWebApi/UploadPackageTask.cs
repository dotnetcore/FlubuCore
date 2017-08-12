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

	    public UploadPackageTask(IWebApiClient client)
	    {
		    _webApiClient = client;
	    }

	    public UploadPackageTask DirectoryPath(string directoryPath)
	    {
		    this._directoryPath = directoryPath;
		    return this;
	    }

	    public UploadPackageTask PackageSearchPattern(string packageSearchPattern)
	    {
		    _packageSearchPattern = packageSearchPattern;
		    return this;
	    }

		protected override int DoExecute(ITaskContextInternal context)
	    {
		    Task<int> task = DoExecuteAsync(context);

		    return task.GetAwaiter().GetResult();
	    }

	    protected override async Task<int> DoExecuteAsync(ITaskContextInternal context)
	    {
		    await _webApiClient.UploadPackageAsync(new UploadPackageRequest
		    {
			    PackageSearchPattern = _packageSearchPattern,
				DirectoryPath = _directoryPath
		    });

		    return 0;
	    }
	}
}

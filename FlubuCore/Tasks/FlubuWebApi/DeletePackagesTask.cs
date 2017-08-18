using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.WebApi.Client;

namespace FlubuCore.Tasks.FlubuWebApi
{
    public class DeletePackagesTask : WebApiBaseTask<DeletePackagesTask, int>
    {
	    public DeletePackagesTask(IWebApiClient webApiClient) : base(webApiClient)
	    {
	    }

		protected override int DoExecute(ITaskContextInternal context)
		{
			Task<int> task = DoExecuteAsync(context);

			return task.GetAwaiter().GetResult();
		}

		protected override async Task<int> DoExecuteAsync(ITaskContextInternal context)
	    {
		    PrepareWebApiClient(context);
		    await WebApiClient.DeletePackagesAsync();
		    return 0;
	    }
	}
}

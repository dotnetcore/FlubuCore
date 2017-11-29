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
        private string _description;

        public DeletePackagesTask(IWebApiClient webApiClient) : base(webApiClient)
	    {
	    }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Deletes all previously uploaded packages from flubu server '{WebApiClient.WebApiBaseUrl}'.";
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
		    await WebApiClient.DeletePackagesAsync();
		    return 0;
	    }
	}
}

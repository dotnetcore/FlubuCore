using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.WebApi.Client;
using FlubuCore.WebApi.Model;

namespace FlubuCore.Tasks.FlubuWebApi
{
    public class GetTokenTask : WebApiBaseTask<GetTokenTask, string>
    {
	    private string _username;

	    private string _password;

	    public GetTokenTask(string username, string password, IWebApiClient webApiClient) : base(webApiClient)
	    {
		    _username = username;
		    _password = password;

	    }

	    protected override string DoExecute(ITaskContextInternal context)
	    {
			Task<string> task = DoExecuteAsync(context);

		    return task.GetAwaiter().GetResult();
		}

	    protected override async Task<string> DoExecuteAsync(ITaskContextInternal context)
	    {
		    PrepareWebApiClient(context);

		    var response = await WebApiClient.GetToken(new GetTokenRequest
		    {
			    Username = _username,
				Password = _password
		    });
		    WebApiClient.Token = response.Token;
		    return response.Token;
	    }
    }
}

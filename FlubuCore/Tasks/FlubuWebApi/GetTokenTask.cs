using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.WebApi.Client;
using FlubuCore.WebApi.Model;

namespace FlubuCore.Tasks.FlubuWebApi
{
    public class GetTokenTask : WebApiBaseTask<GetTokenTask, string>
    {
        private readonly string _password;
        private readonly string _username;
        private string _description;

        public GetTokenTask(string username, string password, IWebApiClient webApiClient)
            : base(webApiClient)
        {
            _username = username;
            _password = password;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                    return $"Get's access token for flubu server {WebApiClient.WebApiBaseUrl}";

                return _description;
            }

            set => _description = value;
        }

        protected override string DoExecute(ITaskContextInternal context)
        {
            var task = DoExecuteAsync(context);

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
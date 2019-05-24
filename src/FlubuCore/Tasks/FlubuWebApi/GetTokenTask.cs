using System;
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
        private string _webApiUrl;
        private TimeSpan? _timeout;
        private string _description;

        public GetTokenTask(string username, string password, IWebApiClientFactory webApiClient)
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
                    return $"Get's access token for flubu server.";

                return _description;
            }

            set => _description = value;
        }

        /// <summary>
        /// Set's web api base url on web api client.
        /// </summary>
        /// <param name="webApiUrl"></param>
        /// <returns></returns>
        public GetTokenTask SetWebApiBaseUrl(string webApiUrl)
        {
            _webApiUrl = webApiUrl;
            return this;
        }

        /// <summary>
        /// Set's timeout on web api client.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public GetTokenTask SetTimeout(TimeSpan timeout)
        {
            _timeout = timeout;
            return this;
        }

        protected override string DoExecute(ITaskContextInternal context)
        {
            var task = DoExecuteAsync(context);

            return task.GetAwaiter().GetResult();
        }

        protected override async Task<string> DoExecuteAsync(ITaskContextInternal context)
        {
            if (string.IsNullOrWhiteSpace(_webApiUrl))
            {
                _webApiUrl = context.Properties.GetFlubuWebApiBaseUrl();
                if (string.IsNullOrWhiteSpace(_webApiUrl))
                {
                    throw new TaskExecutionException("Web api base url not set. Set it through GetTokenTask or put base url in build properties session.", 25);
                }
            }

            if (!_webApiUrl.EndsWith("/"))
            {
                _webApiUrl = $"{_webApiUrl}/";
            }

            context.Properties.Set(BuildProps.LastWebApiBaseUrl, _webApiUrl);
            var client = WebApiClientFactory.Create(_webApiUrl);

            if (_timeout.HasValue)
            {
                client.Timeout = _timeout.Value;
            }

            var response = await client.ExecuteAsync(c => c.GetTokenAsync(new GetTokenRequest
            {
                Username = _username,
                Password = _password
            }));

            if (response.Error != null)
            {
               throw new TaskExecutionException($"Get token failed: ErrorCode: {response.Error.ErrorCode} ErrorMessage: {response.Error.ErrorMessage}", 99);
            }

            client.Token = response.Data.Token;
            return response.Data.Token;
        }
    }
}
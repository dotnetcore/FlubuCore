using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.WebApi.Client.Attributes;
using FlubuCore.WebApi.Model;

namespace FlubuCore.WebApi.Client
{
    public class WebApiClient : RestClient, IWebApiClient
    {
        public WebApiClient(HttpClient client) : base(client)
        {
        }

        [Post("api/scripts/execute")]
        public async Task ExecuteScriptAsync(ExecuteScriptRequest request)
        {
            await SendAsync(request);
        }
    }
}

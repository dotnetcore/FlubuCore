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
        public WebApiClient(ClientSettings settings) : base(settings)
        {
        }

        public WebApiClient(HttpClient httpClient, ClientSettings settings = null) : base(httpClient, settings)
        {
        }

        [Post("api/scripts/execute")]
        public async Task ExecuteScript(ExecuteScriptRequest request)
        {
            await SendAsync(request);
        }
    }
}

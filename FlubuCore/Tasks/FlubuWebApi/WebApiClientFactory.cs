using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Infrastructure;
using FlubuCore.WebApi.Client;

namespace FlubuCore.Tasks.FlubuWebApi
{
    public class WebApiClientFactory : IWebApiClientFactory
    {
        private static IDictionary<string, IWebApiClient> _clients = new Dictionary<string, IWebApiClient>();

        private readonly IHttpClientFactory _httpClientFactory;

        public WebApiClientFactory(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IWebApiClient Create(string endpoint)
        {
            IWebApiClient client;

            if (_clients.TryGetValue(endpoint, out client))
            {
                return client;
            }

            client = new WebApiClient(_httpClientFactory.Create(endpoint));
            _clients[endpoint] = client;

            return client;
        }
    }
}

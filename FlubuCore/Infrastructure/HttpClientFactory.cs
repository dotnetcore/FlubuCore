using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace FlubuCore.Infrastructure
{
    public class HttpClientFactory : IHttpClientFactory
    {
        private static IDictionary<string, HttpClient> _clients = new Dictionary<string, HttpClient>();

        public HttpClient Create(string endpoint)
        {
            HttpClient client;

            if (_clients.TryGetValue(endpoint, out client))
            {
                return client;
            }

            client = new HttpClient
            {
                BaseAddress = new Uri(endpoint),
            };

            _clients[endpoint] = client;

            return client;
        }
    }
}

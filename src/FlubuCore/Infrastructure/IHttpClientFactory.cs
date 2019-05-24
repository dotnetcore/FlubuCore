using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace FlubuCore.Infrastructure
{
    public interface IHttpClientFactory
    {
        HttpClient Create(string endpoint);
    }
}

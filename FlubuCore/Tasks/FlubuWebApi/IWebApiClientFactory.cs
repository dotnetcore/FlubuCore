using FlubuCore.WebApi.Client;

namespace FlubuCore.Tasks.FlubuWebApi
{
    public interface IWebApiClientFactory
    {
        IWebApiClient Create(string endpoint);
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.WebApi.Model;

namespace FlubuCore.WebApi.Client
{
    public interface IWebApiClient
    {
        string WebApiBaseUrl { get; set; }

        TimeSpan Timeout { get; set; }

		string Token { get; set; }

        Task ExecuteScriptAsync(ExecuteScriptRequest request);

        Task UploadPackageAsync(UploadPackageRequest request);

        Task DeletePackagesAsync();

        Task<GetTokenResponse> GetToken(GetTokenRequest request);
    }
}

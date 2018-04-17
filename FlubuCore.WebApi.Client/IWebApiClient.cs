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

        /// <summary>
        /// Executes specified flubu scrip on flubu web api server.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ExecuteScriptResponse> ExecuteScriptAsync(ExecuteScriptRequest request);

        Task UploadScriptAsync(UploadScriptRequest request);

        /// <summary>
        /// Upload's sprecified packages to flubu web api server.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task UploadPackageAsync(UploadPackageRequest request);

        /// <summary>
        /// Cleanes packages directory on flubu web api server.
        /// </summary>
        /// <returns></returns>
        Task DeletePackagesAsync(CleanPackagesDirectoryRequest request);

        /// <summary>
        /// Get's the access token for flubu web api server.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The access token</returns>
        Task<GetTokenResponse> GetToken(GetTokenRequest request);
    }
}

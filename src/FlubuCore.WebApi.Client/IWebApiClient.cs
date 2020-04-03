using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
        /// <returns>List of uploaded packages(files).</returns>
        Task<List<string>> UploadPackageAsync(UploadPackageRequest request);

        /// <summary>
        /// Cleanes packages directory on flubu web api server.
        /// </summary>
        /// <returns></returns>
        Task DeletePackagesAsync(CleanPackagesDirectoryRequest request);

        /// <summary>
        /// Get's the access token for flubu web api server.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The access token.</returns>
        Task<GetTokenResponse> GetTokenAsync(GetTokenRequest request);

        Task HealthCheckAsync();

        /// <summary>
        /// Download reports(compressed in zip file) from flubu web api server.
        /// </summary>
        /// <returns></returns>
        Task<Stream> DownloadReportsAsync(DownloadReportsRequest request);

        /// <summary>
        /// Deletes all reports(cleans directory on flubu web api server).
        /// </summary>
        /// <returns></returns>
        Task CleanReportsDirectoryAsync(CleanReportsDirectoryRequest request);

        /// <summary>
        /// Executes specified web api method and handles <see cref="WebApiException"/> all errors.
        /// </summary>
        /// <typeparam name="T">The response data</typeparam>
        /// <param name="action"></param>
        /// <param name="statusCodeToHandle"></param>
        /// <returns></returns>
        Task<Response<T>> ExecuteAsync<T>(Func<IWebApiClient, Task<T>> action);

        /// <summary>
        /// Executes specified web api method and handles <see cref="WebApiException"/> with specified HttpStatusCode. If WebException with other status code has occured exception is retrown.
        /// </summary>
        /// <typeparam name="T">The response data.</typeparam>
        /// <param name="action"></param>
        /// <param name="statusCodesToHandle">HttpStatusCodes to handle.</param>
        /// <returns></returns>
        Task<Response<T>> ExecuteAsync<T>(Func<IWebApiClient, Task<T>> action, params HttpStatusCode[] statusCodesToHandle);

        /// <summary>
        /// Executes specified web api method and handles <see cref="WebApiException"/> with specified HttpStatusCode. If WebException with other status code has occured exception is retrown.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="statusCodeToHandle"></param>
        /// <returns></returns>
        Task<ErrorModel> ExecuteAsync(Func<IWebApiClient, Task> action, HttpStatusCode statusCodeToHandle = HttpStatusCode.BadRequest);

        /// <summary>
        /// Executes specified web api method and handles <see cref="WebApiException"/> with specified HttpStatusCode. If WebException with other status code has occured exception is retrown.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="statusCodesToHandle">HttpStatusCodes to handle.</param>
        /// <returns></returns>
        Task<ErrorModel> ExecuteAsync(Func<IWebApiClient, Task> action, params HttpStatusCode[] statusCodesToHandle);

        /// <summary>
        /// Handles errors in execute method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        void HandleErrors<T>(Response<T> response);
    }
}

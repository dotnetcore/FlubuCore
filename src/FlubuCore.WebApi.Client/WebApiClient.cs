using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.WebApi.Client.Attributes;
using FlubuCore.WebApi.Model;
using Newtonsoft.Json;

namespace FlubuCore.WebApi.Client
{
    public class WebApiClient : RestClient, IWebApiClient
    {
        public WebApiClient(HttpClient client)
            : base(client)
        {
        }

        [Post("api/scripts/execute")]
        public async Task<ExecuteScriptResponse> ExecuteScriptAsync(ExecuteScriptRequest request)
        {
           return await SendAsync<ExecuteScriptResponse>(request);
        }

        public async Task UploadScriptAsync(UploadScriptRequest request)
        {
            if (!File.Exists(request.FilePath))
            {
                return;
            }

            using (var content = new MultipartFormDataContent())
            {
                ////todo investigate why one content has to be added.
                content.Add(new ByteArrayContent(new byte[0]), "fake");

                var stream = new FileStream(request.FilePath, FileMode.Open);
                string fileName = Path.GetFileName(request.FilePath);
                content.Add(new StreamContent(stream), fileName, fileName);

                Client.DefaultRequestHeaders.Authorization = !string.IsNullOrEmpty(Token)
                    ? new AuthenticationHeaderValue("Bearer", Token)
                    : null;
                var response = await Client.PostAsync(new Uri(string.Format("{0}api/scripts/upload", WebApiBaseUrl)),
                    content);

                await GetResponse<Void>(response);
            }
        }

        public async Task<List<string>> UploadPackageAsync(UploadPackageRequest request)
        {
            FileInfo[] filesInDir;
            DirectoryInfo directoryInWhichToSearch = new DirectoryInfo(request.DirectoryPath);
            if (!string.IsNullOrEmpty(request.PackageSearchPattern))
            {
                filesInDir = directoryInWhichToSearch.GetFiles(request.PackageSearchPattern);
            }
            else
            {
                filesInDir = directoryInWhichToSearch.GetFiles();
            }

            if (filesInDir.Length == 0)
            {
                return new List<string>();
            }

            List<string> uploadedFiles = new List<string>();
            using (var content = new MultipartFormDataContent())
            {
                ////todo investigate why one content has to be added.
                var json = JsonConvert.SerializeObject(request);
                content.Add(new StringContent(json), "request");
                foreach (var file in filesInDir)
                {
                    uploadedFiles.Add(file.FullName);
                    var stream = new FileStream(file.FullName, FileMode.Open);
                    string fileName = Path.GetFileName(file.FullName);
                    content.Add(new StreamContent(stream), fileName, fileName);
                }

                Client.DefaultRequestHeaders.Authorization = !string.IsNullOrEmpty(Token)
                    ? new AuthenticationHeaderValue("Bearer", Token)
                    : null;
                var response = await Client.PostAsync(new Uri(string.Format("{0}api/packages", WebApiBaseUrl)), content);

                await GetResponse<Void>(response);

                return uploadedFiles;
            }
        }

        [Delete("api/packages")]
        public async Task DeletePackagesAsync(CleanPackagesDirectoryRequest request)
        {
            await SendAsync(request);
        }

        [Post("api/Auth")]
        public async Task<GetTokenResponse> GetTokenAsync(GetTokenRequest request)
        {
            return await SendAsync<GetTokenResponse>(request);
        }

        [Get("api/HealthCheck")]
        public async Task HealthCheckAsync()
        {
            await SendAsync();
        }

        [Post("api/reports/download")]
        public async Task<Stream> DownloadReportsAsync(DownloadReportsRequest request)
        {
           return await GetStreamAsync(request);
        }

        [Delete("api/reports/download")]
        public async Task CleanReportsDirectoryAsync(CleanReportsDirectoryRequest request)
        {
            await SendAsync(request);
        }

         /// <summary>
        /// Executes specified web api method and handles <see cref="WebApiException"/> with specified HttpStatusCode. If WebException with other status code has occured exception is retrown.
        /// </summary>
        /// <typeparam name="T">The response data.</typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<Response<T>> ExecuteAsync<T>(Func<IWebApiClient, Task<T>> action)
        {
          return await ExecuteAsync(action, null);
        }

        /// <summary>
        /// Executes specified web api method and handles <see cref="WebApiException"/> <see cref="TimeoutException"/> with specified HttpStatusCodes. If WebException with other status code has occured exception is rethrown.
        /// </summary>
        /// <typeparam name="T">The response data.</typeparam>
        /// <param name="action"></param>
        /// <param name="statusCodesToHandle">HttpStatusCodes to handle.</param>
        /// <returns></returns>
        public async Task<Response<T>> ExecuteAsync<T>(Func<IWebApiClient, Task<T>> action, params HttpStatusCode[] statusCodesToHandle)
        {
            try
            {
                T result = await action(this);
                return new Response<T> { Data = result };
            }
            catch (WebApiException ex)
            {
                if (statusCodesToHandle == null || statusCodesToHandle.Contains(ex.StatusCode))
                {
                    var response = new Response<T> { Error = ex.ErrorModel };
                    HandleErrors(response);
                    return response;
                }

                throw;
            }
            catch (TaskCanceledException ex)
            {
                var response = new Response<T>
                {
                    Error = new ErrorModel
                    {
                        ErrorMessage = ex.Message,
                        StackTrace = ex.StackTrace,
                    }
                };
                HandleErrors(response);
                return response;
            }
            catch (TimeoutException ex)
            {
                var response = new Response<T>
                {
                    Error = new ErrorModel
                    {
                        ErrorMessage = ex.Message,
                        StackTrace = ex.StackTrace,
                    }
                };
                HandleErrors(response);
                return response;
            }
        }

        /// <summary>
        /// Executes specified web api method and handles <see cref="WebApiException"/> with specified HttpStatusCode. If WebException with other status code has occured exception is rethrown.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<ErrorModel> ExecuteAsync(Func<IWebApiClient, Task> action, HttpStatusCode statusCodeToHandle = HttpStatusCode.BadRequest)
        {
            return await ExecuteAsync(action, new HttpStatusCode[] { statusCodeToHandle });
        }

        /// <summary>
        /// Executes specified web api method and handles <see cref="WebApiException"/> with specified HttpStatusCode. If WebException with other status code has occured exception is rethrown.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="statusCodesToHandle">HttpStatusCodes to handle.</param>
        /// <returns></returns>
        public async Task<ErrorModel> ExecuteAsync(Func<IWebApiClient, Task> action, params HttpStatusCode[] statusCodesToHandle)
        {
            try
            {
                await action(this);
                return null;
            }
            catch (WebApiException ex)
            {
                var statusCodes = statusCodesToHandle.ToList();
                if (statusCodes.Contains(ex.StatusCode))
                {
                    return ex.ErrorModel;
                }

                throw;
            }
        }

        /// <summary>
        /// Handles errors in execute method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        public virtual void HandleErrors<T>(Response<T> response)
        {
        }
    }
}

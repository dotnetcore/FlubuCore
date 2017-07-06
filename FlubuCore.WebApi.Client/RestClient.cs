using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.WebApi.Client.Attributes;
using Newtonsoft.Json;

namespace FlubuCore.WebApi.Client
{
    public abstract class RestClient
    {
        private List<MethodInfo> methods = null;

        internal RestClient(ClientSettings settings)
        {
            Client = new HttpClient();
            Settings = settings;
            Client.Timeout = TimeSpan.FromMilliseconds(settings.Timeout);
            GetAllClientMethods();
        }

        internal RestClient(HttpClient httpClient, ClientSettings settings = null)
        {
            Client = httpClient;
            this.Settings = settings ?? new ClientSettings();
            Settings.BaseUrl = httpClient.BaseAddress.ToString();
            GetAllClientMethods();
        }

        internal async Task<TResponse> SendAsync<TResponse>([CallerMemberName] string memberName = "") where TResponse : new()
        {
            return await SendAsync<TResponse>(null, memberName);
        }

        internal async Task SendAsync(object request, [CallerMemberName] string memberName = "", string queryString = null, bool deleteFromBody = false)
        {
            await SendAsync<Void>(request, memberName, queryString);
        }

        internal async Task SendAsync([CallerMemberName] string memberName = "")
        {
            await SendAsync<Void>(null, memberName);
        }

        internal async Task<TResponse> SendAsync<TResponse>(object request, [CallerMemberName]string memberName = "", string queryString = null) where TResponse : new()
        {
            var method = methods.FirstOrDefault(x => x.Name == memberName);
            var attribute = method.GetCustomAttribute<HttpAttribute>();
            var relativePath = UrlHelpers.ReplaceParameterTemplatesInRelativePathWithValues(attribute.Path, request);

            HttpRequestMessage requestMessage = new HttpRequestMessage();
            requestMessage.Method = attribute.Method;

            if (attribute.Method == HttpMethod.Post || attribute.Method == HttpMethod.Put || (attribute.Method == HttpMethod.Delete))
            {
                if (request != null)
                {
                    var jsonObject = JsonConvert.SerializeObject(request);
                    StringContent content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                    requestMessage.Content = content;
                }

                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                ////Client.DefaultRequestHeaders.Authorization = !string.IsNullOrEmpty(Token) ? new AuthenticationHeaderValue("Bearer", Token) : null;
            }
            else
            {
                ////Client.DefaultRequestHeaders.Authorization = !string.IsNullOrEmpty(Token) ? new AuthenticationHeaderValue("Bearer", Token) : null;

                if (request != null && queryString == null)
                {
                    queryString = string.Format("?{0}", request.ToQueryString());
                }
            }

            var uri = new Uri(string.Format("{0}{1}{2}", Settings.BaseUrl, relativePath, queryString));
            requestMessage.RequestUri = uri;
            var responseMessage = await Client.SendAsync(requestMessage);
            return await GetResponse<TResponse>(responseMessage);
        }

        private async Task<T> GetResponse<T>(HttpResponseMessage response) where T : new()
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var errorString = await response.Content.ReadAsStringAsync();
                throw new WebApiException(response.StatusCode, errorString);
            }

            if (typeof(T) == typeof(Void))
            {
                var tcs = new TaskCompletionSource<T>();
                tcs.SetResult(new T());
                return await tcs.Task;
            }

            var jsonString = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<T>(jsonString);
            return result;
        }

        protected virtual void GetAllClientMethods()
        {
            methods = GetType().GetRuntimeMethods().ToList();
        }

        public ClientSettings Settings { get; set; }

        protected HttpClient Client { get; set; }
    }
}
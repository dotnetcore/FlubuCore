using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.WebApi.Client.Attributes;
using FlubuCore.WebApi.Model;

namespace FlubuCore.WebApi.Client
{
    public class WebApiClient : RestClient, IWebApiClient
    {
        public WebApiClient(HttpClient client) : base(client)
        {
        }

        [Post("api/scripts/execute")]
        public async Task ExecuteScriptAsync(ExecuteScriptRequest request)
        {
            await SendAsync(request);
        }

        public async Task UploadPackageAsync(UploadPackageRequest request)
        {
            FileInfo[] filesInDir;
            DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(request.DirectoryPath);
            if (!string.IsNullOrEmpty(request.PackageSearchPattern))
            {

                filesInDir = hdDirectoryInWhichToSearch.GetFiles(request.PackageSearchPattern);
            }
            else

            {
                filesInDir = hdDirectoryInWhichToSearch.GetFiles();
            }


            if (filesInDir.Length == 0)
            {
                return;
            }

            using (var content = new MultipartFormDataContent())
            {
                foreach (var file in filesInDir)
                {
                    var stream = new FileStream(file.FullName, FileMode.Open);
                    string fileName = Path.GetFileName(file.FullName);
                    content.Add(new StreamContent(stream), fileName, fileName);
                }

                var response = await Client.PostAsync(new Uri(string.Format("{0}api/packages/upload", WebApiBaseUrl)), content);

                await GetResponse<Void>(response);
            }
        }
    }
}

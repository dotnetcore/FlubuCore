using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FlubuCore.Infrastructure
{
    public static class HttpClientExtensions
    {
        public static async Task DownloadFileAsync(this HttpClient client, string url, string fileName)
        {
            using (var response = await client.GetAsync(url))
            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var file = File.OpenWrite(fileName))
            {
               await stream.CopyToAsync(file);
            }
        }
    }
}

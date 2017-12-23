using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.WebApi.Configuration;
using FlubuCore.WebApi.Models;
using FlubuCore.WebApi.Repository.Exceptions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FlubuCore.WebApi.Repository
{
    public class SecurityRepository : ISecurityRepository
    {
        private const string FileName = "Security.Json";

        private readonly WebApiSettings _webApiSettings;

        public SecurityRepository(IOptions<WebApiSettings> webApiSettings)
        {
            _webApiSettings = webApiSettings.Value;
            if (!File.Exists(FileName))
            {
                Security security = new Security();
                var json = JsonConvert.SerializeObject(security);
                File.WriteAllText(FileName, json);
            }
        }

        public async Task<Security> GetSecurityAsync()
        {
            using (FileStream fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader r = new StreamReader(fileStream))
                {
                    string json = await r.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<Security>(json);
                }
            }
        }

        public void IncreaseFailedGetTokenAttempts(Security security)
        {
            security.FailedGetTokenAttempts++;
            if (security.FailedGetTokenAttempts >= _webApiSettings.MaxFailedLoginAttempts)
            {
                security.ApiAccessDisabled = true;
                security.FailedGetTokenAttempts = 0;
            }

            var json = JsonConvert.SerializeObject(security);
            File.WriteAllText(FileName, json);
        }
    }
}

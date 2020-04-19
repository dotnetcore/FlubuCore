using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.WebApi.Configuration;
using FlubuCore.WebApi.Models;
using FlubuCore.WebApi.Repository.Exceptions;
using LiteDB;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using FileMode = System.IO.FileMode;

namespace FlubuCore.WebApi.Repository
{
    public class SecurityRepository : ISecurityRepository
    {
        private const string FileName = "Security.Json";

        private readonly WebApiSettings _webApiSettings;

        private readonly LiteRepository _repository;

        public SecurityRepository(IOptions<WebApiSettings> webApiSettings, LiteRepository repository)
        {
            _repository = repository;
            _webApiSettings = webApiSettings.Value;
        }

        public Security GetSecurity()
        {
            var security = _repository.FirstOrDefault<Security>(x => x != null);
            if (security != null)
            {
                return security;
            }

            security = new Security();
            _repository.Insert(security, "security");
            return security;
        }

        public void IncreaseFailedGetTokenAttempts(Security security)
        {
            security.FailedGetTokenAttempts++;
            if (security.FailedGetTokenAttempts >= _webApiSettings.MaxFailedLoginAttempts)
            {
                security.ApiAccessDisabled = true;
                security.FailedGetTokenAttempts = 0;
            }

            _repository.Update(security);
        }
    }
}

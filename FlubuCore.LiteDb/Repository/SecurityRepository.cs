namespace FlubuCore.LiteDb.Repository
{
    using FlubuCore.LiteDb.Models;

    using LiteDB;
    using Microsoft.Extensions.Options;

    public class SecurityRepository : ISecurityRepository
    {
        private const string FileName = "Security.Json";

        private readonly LiteDbSettings _liteDbSettings;

        private readonly LiteRepository _repository;

        public SecurityRepository(IOptions<LiteDbSettings> webApiSettings, LiteRepository repository)
        {
            _repository = repository;
            _liteDbSettings = webApiSettings.Value;
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
            if (security.FailedGetTokenAttempts >= _liteDbSettings.MaxFailedLoginAttempts)
            {
                security.ApiAccessDisabled = true;
                security.FailedGetTokenAttempts = 0;
            }

            _repository.Update(security);
        }
    }
}

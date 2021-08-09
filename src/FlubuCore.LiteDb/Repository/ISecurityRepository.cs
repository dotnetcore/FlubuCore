namespace FlubuCore.LiteDb.Repository
{
    using FlubuCore.LiteDb.Models;

    public interface ISecurityRepository
    {
        Security GetSecurity();

        void IncreaseFailedGetTokenAttempts(Security security);
    }
}

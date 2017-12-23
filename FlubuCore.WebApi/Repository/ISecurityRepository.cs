using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.WebApi.Models;

namespace FlubuCore.WebApi.Repository
{
    public interface ISecurityRepository
    {
        Task<Security> GetSecurityAsync();

        void IncreaseFailedGetTokenAttempts(Security security);
    }
}

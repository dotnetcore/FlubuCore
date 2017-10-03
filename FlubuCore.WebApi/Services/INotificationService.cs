using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.WebApi.Services
{
    public interface INotificationService
    {
        Task SendEmailAsync(string subject, string body);
    }
}

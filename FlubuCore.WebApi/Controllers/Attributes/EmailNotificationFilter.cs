using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.WebApi.Configuration;
using FlubuCore.WebApi.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace FlubuCore.WebApi.Controllers.Attributes
{
    public class EmailNotificationFilter : TypeFilterAttribute
    {
        public EmailNotificationFilter(params NotificationFilter[] notificationFilters)
            : base(typeof(EmailNotificationFilterImpl))
        {
            Arguments = new object[] { notificationFilters };
        }
    }

    public class EmailNotificationFilterImpl : ActionFilterAttribute
    {
        private readonly INotificationService _notificationService;

        private readonly WebApiSettings _webApiSettings;

        private readonly List<NotificationFilter> _notificationFilters;

        public EmailNotificationFilterImpl(INotificationService notificationService, IOptions<WebApiSettings> webApiOptions, NotificationFilter[] notificationFilters)
        {
            _notificationService = notificationService;
            _webApiSettings = webApiOptions.Value;
            _notificationFilters = notificationFilters.ToList();
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!_webApiSettings.SecurityNotificationsEnabled)
            {
                await next();
                return;
            }

            if (_webApiSettings.NotificationFilters != null &&
                _webApiSettings.NotificationFilters.Count != 0 &&
                !_notificationFilters.Any(x => _webApiSettings.NotificationFilters.Any(y => y == x)))
            {
                await next();
                return;
            }

            var emailTask = _notificationService.SendEmailAsync("Flubu security notification",
                $"Resource: '{context.HttpContext.Request.GetDisplayUrl()}' was accessed.");

            await next();
            await emailTask;
        }
    }
}

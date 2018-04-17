using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FlubuCore.WebApi.Configuration;
using FlubuCore.WebApi.Controllers.Exceptions;
using FlubuCore.WebApi.Infrastructure;
using FlubuCore.WebApi.Model;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace FlubuCore.WebApi.Controllers.Attributes
{
    public class RestrictApiAccessFilter : ActionFilterAttribute
    {
        private readonly WebApiSettings _webApiSettings;

        private readonly ITimeProvider _timeProvider;

        public RestrictApiAccessFilter(IOptions<WebApiSettings> webApiOptions, ITimeProvider timeProvider)
        {
            _webApiSettings = webApiOptions.Value;
            _timeProvider = timeProvider;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (_webApiSettings.AllowedIps != null && _webApiSettings.AllowedIps.Count != 0)
            {
                string clientIp = context.HttpContext.Connection.RemoteIpAddress.ToString();
                if (!_webApiSettings.AllowedIps.Contains(clientIp))
                {
                    throw new HttpError(HttpStatusCode.Forbidden);
                }
            }

            if (_webApiSettings.TimeFrames != null && _webApiSettings.TimeFrames.Count > 0)
            {
                var now = _timeProvider.Now.TimeOfDay;
                bool timeFrameMatch =
                    _webApiSettings.TimeFrames.Any(timeFrame => timeFrame.TimeFrom < now && timeFrame.TimeTo > now);

                if (!timeFrameMatch)
                {
                    throw new HttpError(HttpStatusCode.Forbidden);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FlubuCore.WebApi.Configuration;
using FlubuCore.WebApi.Controllers.Exception;
using FlubuCore.WebApi.Model;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace FlubuCore.WebApi.Controllers.Attributes
{
	public class RestrictApiAccessFilter : ActionFilterAttribute
	{
		private readonly WebApiSettings _webApiSettings;
		public RestrictApiAccessFilter(IOptions<WebApiSettings> webApiOptions)
		{
			_webApiSettings = webApiOptions.Value;
		}

		public override void OnActionExecuting(ActionExecutingContext context)
		{
		   if (_webApiSettings.AllowedIps != null && _webApiSettings.AllowedIps.Count != 0)
		   {
			   string clientIp = context.HttpContext.Connection.RemoteIpAddress.ToString();
			   if (!_webApiSettings.AllowedIps.Contains(clientIp))
			   {
				   throw new HttpError(HttpStatusCode.Forbidden, ErrorCodes.FordibenIp);
			   }
		   }
		}
	}
}

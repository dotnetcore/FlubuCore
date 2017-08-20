using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;
using FlubuCore.WebApi.Configuration;
using FlubuCore.WebApi.Controllers;
using FlubuCore.WebApi.Controllers.Attributes;
using FlubuCore.WebApi.Controllers.Exception;
using FlubuCore.WebApi.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Moq;
using NuGet.Packaging;
using Xunit;

namespace FlubuCore.WebApi.Tests.AttributeTests
{
    public class RestrictApiAccessFilterTests
    {
	    private RestrictApiAccessFilter filter;

	    private IOptions<WebApiSettings> settingOptions;

	    private Mock<ITimeProvider> timeProvider;

	    private ActionExecutingContext context;

	    private WebApiSettings webApiSettings;

		public RestrictApiAccessFilterTests()
	    {
			context = new ActionExecutingContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), new List<IFilterMetadata>(), new ConcurrentDictionary<string, object>(), new ScriptsController(null, null, null, null));
			timeProvider = new Mock<ITimeProvider>();
			webApiSettings = new WebApiSettings();
			settingOptions = new OptionsWrapper<WebApiSettings>(webApiSettings);
		    filter = new RestrictApiAccessFilter(settingOptions, timeProvider.Object);
	    }

	    [Fact]
	    public void SettingsNull_Succesfull()
	    {
			filter.OnActionExecuting(context);
		}

	    [Fact]
	    public void SettingsEmpty_Succesfull()
	    {
			webApiSettings.TimeFrames = new List<TimeFrame>();
			webApiSettings.AllowedIps = new List<string>();
		    filter.OnActionExecuting(context);
	    }

		[Fact]
	    public void IpOnWhiteList_Succesfull()
	    {
			context.HttpContext.Connection.RemoteIpAddress = new IPAddress(0x2414188f);
			webApiSettings.AllowedIps = new List<string>
			{
				"143.24.20.36"
			};
			filter.OnActionExecuting(context);
		}

	    [Fact]
	    public void IpNotOnWhiteList_ThrowsForbiden()
	    {
		    context.HttpContext.Connection.RemoteIpAddress = new IPAddress(0x2414188f);
		    webApiSettings.AllowedIps = new List<string>
		    {
			    "143.24.20.35"
		    };

			var exception = Assert.Throws<HttpError>(() => filter.OnActionExecuting(context));
		    Assert.Equal(HttpStatusCode.Forbidden, exception.StatusCode);
		}

		[Theory]
		[InlineData(20, 0, 1)]
	    [InlineData(21, 0, 0)]
	    [InlineData(23, 59, 59)]
	    [InlineData(2, 14, 00)]
		public void InsideTimeFrame_Succesfull(int hour, int min, int sec)
	    {
		    webApiSettings.TimeFrames = new List<TimeFrame>()
		    {
			    new TimeFrame
			    {
				    TimeFrom = new TimeSpan(2, 0, 0),
				    TimeTo = new TimeSpan(2, 15, 0)
			    },
				new TimeFrame
			    {
				    TimeFrom = new TimeSpan(20, 0, 0),
				    TimeTo = new TimeSpan(24, 0, 0)
			    }
			};

		    timeProvider.Setup(x => x.Now).Returns(new DateTime(2000, 1, 1, hour, min, sec, 0));
			filter.OnActionExecuting(context);
	    }


	    [Theory]
	    [InlineData(19, 59, 59)]
	    [InlineData(2, 15, 1)]
	    [InlineData(0, 0, 1)]
	    [InlineData(1, 59, 59)]
	    public void OutsideTimeFrame_ThrowsForbiden(int hour, int min, int sec)
	    {
		    webApiSettings.TimeFrames = new List<TimeFrame>()
		    {
			    new TimeFrame
			    {
				    TimeFrom = new TimeSpan(2, 0, 0),
				    TimeTo = new TimeSpan(2, 15, 0)
			    },
			    new TimeFrame
			    {
				    TimeFrom = new TimeSpan(20, 0, 0),
				    TimeTo = new TimeSpan(24, 0, 0)
			    }
		    };

		    timeProvider.Setup(x => x.Now).Returns(new DateTime(2000, 1, 1, hour, min, sec, 0));

		    var exception = Assert.Throws<HttpError>(() => filter.OnActionExecuting(context));
		    Assert.Equal(HttpStatusCode.Forbidden, exception.StatusCode);
	    }
    }
}

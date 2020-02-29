using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;
using FlubuCore.WebApi.Configuration;
using FlubuCore.WebApi.Controllers;
using FlubuCore.WebApi.Controllers.Attributes;
using FlubuCore.WebApi.Controllers.Exceptions;
using FlubuCore.WebApi.Infrastructure;
using Microsoft.AspNetCore.Http;
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
        private RestrictApiAccessFilter _filter;

        private IOptions<WebApiSettings> _settingOptions;

        private Mock<ITimeProvider> _timeProvider;

        private ActionExecutingContext _context;

        private WebApiSettings _webApiSettings;

        public RestrictApiAccessFilterTests()
        {
            _timeProvider = new Mock<ITimeProvider>();
            _webApiSettings = new WebApiSettings();
            _settingOptions = new OptionsWrapper<WebApiSettings>(_webApiSettings);
            _context = new ActionExecutingContext(
                new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
                new List<IFilterMetadata>(), new ConcurrentDictionary<string, object>(),
                new ScriptsController(null, null, null, null, _settingOptions, null));
            _filter = new RestrictApiAccessFilter(_settingOptions, _timeProvider.Object);
        }

        [Fact]
        public void SettingsNull_Succesfull()
        {
            _filter.OnActionExecuting(_context);
        }

        [Fact]
        public void SettingsEmpty_Succesfull()
        {
            _webApiSettings.TimeFrames = new List<TimeFrame>();
            _webApiSettings.AllowedIps = new List<string>();
            _filter.OnActionExecuting(_context);
        }

        [Fact]
        public void IpOnWhiteList_Succesfull()
        {
            _context.HttpContext.Connection.RemoteIpAddress = new IPAddress(0x2414188f);
            _webApiSettings.AllowedIps = new List<string>
            {
                "143.24.20.36"
            };
            _filter.OnActionExecuting(_context);
        }

        [Fact]
        public void IpNotOnWhiteList_ThrowsForbiden()
        {
            _context.HttpContext.Connection.RemoteIpAddress = new IPAddress(0x2414188f);
            _webApiSettings.AllowedIps = new List<string>
            {
                "143.24.20.35"
            };

            var exception = Assert.Throws<HttpError>(() => _filter.OnActionExecuting(_context));
            Assert.Equal(HttpStatusCode.Forbidden, exception.StatusCode);
        }

        [Theory]
        [InlineData(20, 0, 1)]
        [InlineData(21, 0, 0)]
        [InlineData(23, 59, 59)]
        [InlineData(2, 14, 00)]
        public void InsideTimeFrame_Succesfull(int hour, int min, int sec)
        {
            _webApiSettings.TimeFrames = new List<TimeFrame>()
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

            _timeProvider.Setup(x => x.Now).Returns(new DateTime(2000, 1, 1, hour, min, sec, 0));
            _filter.OnActionExecuting(_context);
        }

        [Theory]
        [InlineData(19, 59, 59)]
        [InlineData(2, 15, 1)]
        [InlineData(0, 0, 1)]
        [InlineData(1, 59, 59)]
        public void OutsideTimeFrame_ThrowsForbiden(int hour, int min, int sec)
        {
            _webApiSettings.TimeFrames = new List<TimeFrame>()
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

            _timeProvider.Setup(x => x.Now).Returns(new DateTime(2000, 1, 1, hour, min, sec, 0));

            var exception = Assert.Throws<HttpError>(() => _filter.OnActionExecuting(_context));
            Assert.Equal(HttpStatusCode.Forbidden, exception.StatusCode);
        }
    }
}

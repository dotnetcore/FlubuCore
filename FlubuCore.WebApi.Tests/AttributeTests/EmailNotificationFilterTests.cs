using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlubuCore.WebApi.Configuration;
using FlubuCore.WebApi.Controllers;
using FlubuCore.WebApi.Controllers.Attributes;
using FlubuCore.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace FlubuCore.WebApi.Tests.AttributeTests
{
    public class EmailNotificationFilterTests
    {
        private readonly Mock<INotificationService> _notificationService;
        private readonly Mock<IOptions<WebApiSettings>> _webApiOptions;
        private readonly WebApiSettings _webApiSettings;
        private readonly ActionExecutingContext _context;
        private EmailNotificationFilterImpl _filter;

        public EmailNotificationFilterTests()
        {
            _webApiSettings = new WebApiSettings();
            _webApiOptions = new Mock<IOptions<WebApiSettings>>();

            _webApiOptions.Setup(x => x.Value).Returns(_webApiSettings);
            _notificationService = new Mock<INotificationService>(MockBehavior.Strict);
            var httpContext = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            request.Setup(x => x.Host).Returns(new HostString("localhost", 2000));
            request.Setup(x => x.Path).Returns(new PathString("/test"));
            request.Setup(x => x.PathBase).Returns(new PathString("/localhost"));
            request.Setup(x => x.QueryString).Returns(new QueryString("?test=3"));
            request.Setup(x => x.Scheme).Returns("https");
            httpContext.Setup(x => x.Request).Returns(request.Object);
            _context = new ActionExecutingContext(new ActionContext(httpContext.Object, new Mock<RouteData>().Object, new Mock<ActionDescriptor>().Object), new List<IFilterMetadata>(), new ConcurrentDictionary<string, object>(), new Mock<Controller>().Object);
        }

        [Fact]
        public async Task EmailNotificationDisabled()
        {
            _webApiSettings.SecurityNotificationsEnabled = false;
            _filter = new EmailNotificationFilterImpl(_notificationService.Object, _webApiOptions.Object,
                new NotificationFilter[0]);
            await _filter.OnActionExecutionAsync(_context, Target);

            _notificationService.VerifyAll();
        }

        [Fact]
        public async Task EmailNotificationEnabledFilteredOut()
        {
            _webApiSettings.SecurityNotificationsEnabled = true;
            _webApiSettings.NotificationFilters = new List<NotificationFilter>() { NotificationFilter.ExecuteScript };
            _filter = new EmailNotificationFilterImpl(_notificationService.Object, _webApiOptions.Object,
                new[] { NotificationFilter.FailedGetToken });
            await _filter.OnActionExecutionAsync(_context, Target);

            _notificationService.VerifyAll();
        }

        [Fact]
        public async Task EmailNotificationEnabledNoFilters()
        {
            _webApiSettings.SecurityNotificationsEnabled = true;
            _webApiSettings.NotificationFilters = null;
            _filter = new EmailNotificationFilterImpl(_notificationService.Object, _webApiOptions.Object,
                new[] { NotificationFilter.ExecuteScript });
            _notificationService.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            await _filter.OnActionExecutionAsync(_context, Target);

            _notificationService.VerifyAll();
        }

        [Fact]
        public async Task EmailNotificationEnabledNoFilters2()
        {
            _webApiSettings.SecurityNotificationsEnabled = true;
            _webApiSettings.NotificationFilters = new List<NotificationFilter>();
            _filter = new EmailNotificationFilterImpl(_notificationService.Object, _webApiOptions.Object,
                new[] { NotificationFilter.ExecuteScript });
            _notificationService.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            await _filter.OnActionExecutionAsync(_context, Target);

            _notificationService.VerifyAll();
        }

        [Fact]
        public async Task EmailNotificationEnabledFiltersMatches()
        {
            _webApiSettings.SecurityNotificationsEnabled = true;
            _webApiSettings.NotificationFilters = new List<NotificationFilter>() { NotificationFilter.ExecuteScript, NotificationFilter.GetToken };
            _filter = new EmailNotificationFilterImpl(_notificationService.Object, _webApiOptions.Object,
                new[] { NotificationFilter.ExecuteScript });
            _notificationService.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            await _filter.OnActionExecutionAsync(_context, Target);

            _notificationService.VerifyAll();
        }

        private Task<ActionExecutedContext> Target()
        {
            return Task.FromResult(new ActionExecutedContext(
                new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
                new List<IFilterMetadata>(), new PackagesController(null, null)));
        }
    }
}

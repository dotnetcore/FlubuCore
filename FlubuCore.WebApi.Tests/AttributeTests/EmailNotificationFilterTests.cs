using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.WebApi.Configuration;
using FlubuCore.WebApi.Controllers;
using FlubuCore.WebApi.Controllers.Attributes;
using FlubuCore.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Moq;
using Renci.SshNet;
using Xunit;

namespace FlubuCore.WebApi.Tests.AttributeTests
{
    public class EmailNotificationFilterTests
    {
        private EmailNotificationFilterImpl filter;

        private Mock<INotificationService> notificationService;

        private Mock<IOptions<WebApiSettings>> webApiOptions;

        private WebApiSettings webApiSettings;

        private ActionExecutingContext context;

        public EmailNotificationFilterTests()
        {
            webApiSettings = new WebApiSettings();
            webApiOptions = new Mock<IOptions<WebApiSettings>>();

            webApiOptions.Setup(x => x.Value).Returns(webApiSettings);
            notificationService  =new Mock<INotificationService>(MockBehavior.Strict);
            var httpContext = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            request.Setup(x => x.Host).Returns(new HostString("localhost", 2000));
            request.Setup(x => x.Path).Returns(new PathString("/test"));
            request.Setup(x => x.PathBase).Returns(new PathString("/localhost"));
            request.Setup(x => x.QueryString).Returns(new QueryString("?test=3"));
            request.Setup(x => x.Scheme).Returns("https");
            httpContext.Setup(x => x.Request).Returns(request.Object);
            context = new ActionExecutingContext(new ActionContext(httpContext.Object, new Mock<RouteData>().Object, new Mock<ActionDescriptor>().Object), new List<IFilterMetadata>(), new ConcurrentDictionary<string, object>(), new Mock<Controller>().Object);
       
        }

        [Fact]
        public async Task EmailNotificationDisabled()
        {
            webApiSettings.SecurityNotificationsEnabled = false;
            filter = new EmailNotificationFilterImpl(notificationService.Object, webApiOptions.Object,
                new NotificationFilter[0]);
            await filter.OnActionExecutionAsync(context, new ActionExecutionDelegate(Target));

            notificationService.VerifyAll();
        }

        [Fact]
        public async Task EmailNotificationEnabledFilteredOut()
        {
            webApiSettings.SecurityNotificationsEnabled = true;
            webApiSettings.NotificationFilters = new List<NotificationFilter>() { NotificationFilter.ExecuteScript };
            filter = new EmailNotificationFilterImpl(notificationService.Object, webApiOptions.Object,
                new NotificationFilter[] { NotificationFilter.FailedGetToken });
            await filter.OnActionExecutionAsync(context, new ActionExecutionDelegate(Target));

            notificationService.VerifyAll();
        }

        [Fact]
        public async Task EmailNotificationEnabledNoFilters()
        {
            webApiSettings.SecurityNotificationsEnabled = true;
            webApiSettings.NotificationFilters = null;
            filter = new EmailNotificationFilterImpl(notificationService.Object, webApiOptions.Object,
                new NotificationFilter[] { NotificationFilter.ExecuteScript });
            notificationService.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            await filter.OnActionExecutionAsync(context, new ActionExecutionDelegate(Target));

            notificationService.VerifyAll();
        }

        [Fact]
        public async Task EmailNotificationEnabledNoFilters2()
        {
            webApiSettings.SecurityNotificationsEnabled = true;
            webApiSettings.NotificationFilters = new List<NotificationFilter>();
            filter = new EmailNotificationFilterImpl(notificationService.Object, webApiOptions.Object,
                new NotificationFilter[] { NotificationFilter.ExecuteScript });
            notificationService.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            await filter.OnActionExecutionAsync(context, new ActionExecutionDelegate(Target));

            notificationService.VerifyAll();
        }


        [Fact]
        public async Task EmailNotificationEnabledFiltersMatches()
        {
            webApiSettings.SecurityNotificationsEnabled = true;
            webApiSettings.NotificationFilters = new List<NotificationFilter>() { NotificationFilter.ExecuteScript, NotificationFilter.GetToken};
            filter = new EmailNotificationFilterImpl(notificationService.Object, webApiOptions.Object,
                new NotificationFilter[] { NotificationFilter.ExecuteScript });
            notificationService.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            await filter.OnActionExecutionAsync(context, new ActionExecutionDelegate(Target));

            notificationService.VerifyAll();
        }

        private async Task<ActionExecutedContext> Target()
        {
            return new ActionExecutedContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), new List<IFilterMetadata>(), new PackagesController(null));
        }
    }
}

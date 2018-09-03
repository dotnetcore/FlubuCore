using System;
using System.Collections.Generic;
using System.Text;
using Flubu.Tests.TestData.BuildScripts;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Infrastructure;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Flubu.Tests
{
    public class ScriptPropertiesSetterTests
    {
        private readonly ITaskSession _taskSession;

        public ScriptPropertiesSetterTests()
        {
            var sp = new ServiceCollection().AddTransient<ITarget, TargetFluentInterface>()
                .AddTransient<ICoreTaskFluentInterface, CoreTaskFluentInterface>()
                .AddTransient<ITaskFluentInterface, TaskFluentInterface>()
                .AddTransient<ILinuxTaskFluentInterface, LinuxTaskFluentInterface>()
                .AddTransient<IIisTaskFluentInterface, IisTaskFluentInterface>()
                .AddTransient<IWebApiFluentInterface, WebApiFluentInterface>()
                .AddSingleton<IHttpClientFactory, HttpClientFactory>()
                .BuildServiceProvider();
            _taskSession = new TaskSession(new Mock<ILogger<TaskSession>>().Object, new TargetTree(null, null), new CommandArguments(), new Mock<ITaskFactory>().Object, new FluentInterfaceFactory(sp), null, null);
        }

        [Fact]
        public void SetPropertiesFromArg_DifferentTypes_Succesfull()
        {
            var buildScript = new BuildScriptWithForArguments();
            _taskSession.ScriptArgs["s"] = "beda";
            _taskSession.ScriptArgs["l"] = "23";
            _taskSession.ScriptArgs["sog"] = "true";
            _taskSession.ScriptArgs["NoAttribute"] = "Noo";
            _taskSession.ScriptArgs["list"] = "a,b,c";
            ScriptProperties.SetPropertiesFromScriptArg(buildScript,  _taskSession);
            Assert.Equal("beda", buildScript.SolutionFileName);
            Assert.Equal(23, buildScript.Level);
            Assert.True(buildScript.StayOrGo);
            Assert.Equal("Noo", buildScript.NoAttribute);
            Assert.Equal(3, buildScript.SomeList.Count);
        }
    }
}

using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Infrastructure;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using FlubuCore.Tests.TestData.BuildScripts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FlubuCore.Tests
{
    public class ScriptPropertiesSetterTests
    {
        private readonly IFlubuSession _flubuSession;

        private readonly IScriptProperties _scriptProperties;

        public ScriptPropertiesSetterTests()
        {
            _scriptProperties = new ScriptProperties();

            var sp = new ServiceCollection().AddTransient<ITarget, TargetFluentInterface>()
                .AddTransient<ICoreTaskFluentInterface, CoreTaskFluentInterface>()
                .AddTransient<ITaskFluentInterface, TaskFluentInterface>()
                .AddTransient<ILinuxTaskFluentInterface, LinuxTaskFluentInterface>()
                .AddTransient<IIisTaskFluentInterface, IisTaskFluentInterface>()
                .AddTransient<IWebApiFluentInterface, WebApiFluentInterface>()
                .AddSingleton<IHttpClientFactory, HttpClientFactory>()

                .BuildServiceProvider();
            _flubuSession = new FlubuSession(new Mock<ILogger<FlubuSession>>().Object, new TargetTree(null, null), new CommandArguments(), new ScriptServiceProvider(sp),  new Mock<ITaskFactory>().Object, new FluentInterfaceFactory(sp), null, null);
        }

        [Fact]
        public void SetPropertiesFromArg_DifferentTypes_Succesfull()
        {
            var buildScript = new BuildScriptWithForArguments();
            _flubuSession.ScriptArgs["s"] = "beda";
            _flubuSession.ScriptArgs["l"] = "23";
            _flubuSession.ScriptArgs["sog"] = "true";
            _flubuSession.ScriptArgs["NoAttribute"] = "Noo";
            _flubuSession.ScriptArgs["list"] = "a,b,c";
            _scriptProperties.InjectProperties(buildScript,  _flubuSession);
            Assert.Equal("beda", buildScript.SolutionFileName);
            Assert.Equal(23, buildScript.Level);
            Assert.True(buildScript.StayOrGo);
            Assert.Equal("Noo", buildScript.NoAttribute);
            Assert.Equal(3, buildScript.SomeList.Count);
        }
    }
}

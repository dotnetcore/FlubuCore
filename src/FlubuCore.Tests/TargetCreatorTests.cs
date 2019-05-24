using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Infrastructure;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using FlubuCore.Tests.TestData.BuildScripts.ForCreateTargetWithAttributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FlubuCore.Tests
{
    public class TargetCreatorTests
    {
        private readonly ITaskSession _taskSession;

        public TargetCreatorTests()
        {
            var sp = new ServiceCollection().AddTransient<ITarget, TargetFluentInterface>()
                .AddTransient<ICoreTaskFluentInterface, CoreTaskFluentInterface>()
                .AddTransient<ITaskFluentInterface, TaskFluentInterface>()
                .AddTransient<ILinuxTaskFluentInterface, LinuxTaskFluentInterface>()
                .AddTransient<IIisTaskFluentInterface, IisTaskFluentInterface>()
                .AddTransient<IWebApiFluentInterface, WebApiFluentInterface>()
                .AddTransient<IGitFluentInterface, GitFluentInterface>()
                .AddTransient<DockerFluentInterface>()
                .AddTransient<IToolsFluentInterface, ToolsFluentInterface>()
                .AddSingleton<IHttpClientFactory, HttpClientFactory>()
                .BuildServiceProvider();
            _taskSession = new TaskSession(new Mock<ILogger<TaskSession>>().Object, new TargetTree(null, null), new CommandArguments(), new Mock<ITaskFactory>().Object, new FluentInterfaceFactory(sp), null, null);
        }

        [Fact]
        public void CreateTargetFromMethodAttributes_AddsTargets_Sucesfull()
        {
            TargetCreator.CreateTargetFromMethodAttributes(new BuildScriptWithTargetsFromAttribute(), _taskSession);
            Assert.Equal(5, _taskSession.TargetTree.TargetCount);
            Assert.True(_taskSession.TargetTree.HasTarget("Target1"));
            Assert.True(_taskSession.TargetTree.HasTarget("Target2"));
            Assert.True(_taskSession.TargetTree.HasTarget("Target3"));
        }

        [Fact]
        public void CreateTargetFromMethodAttributes_NoTargets_Succesfull()
        {
            TargetCreator.CreateTargetFromMethodAttributes(new BuildScriptNoTargetsWithAttribute(), _taskSession);
            Assert.Equal(2, _taskSession.TargetTree.TargetCount);
        }

        [Fact]
        public void CreateTargetFromMethodAttributes_MethodHasNoParameters_ThrowsScriptException()
        {
           var ex = Assert.Throws<ScriptException>(() => TargetCreator.CreateTargetFromMethodAttributes(new BuildScriptMethodHasNoParameters(), _taskSession));
           Assert.Equal("Failed to create target 'Test'. Method 'TestTarget' must have atleast one parameter which must be of type 'ITarget'", ex.Message);
        }

        [Fact]
        public void CreateTargetFromMethodAttributes_MethodAndAttributeParameterCountDoNotMatch_ThrowsScriptException()
        {
            var ex = Assert.Throws<ScriptException>(() => TargetCreator.CreateTargetFromMethodAttributes(new BuildScriptParameterCountNotMatch(), _taskSession));
            Assert.Equal("Failed to create target 'Test'. Method parameters TestTarget do not match count of attribute parametrs.", ex.Message);
        }

        ////[Fact]
        ////public void CreateTargetFromMethodAttributes_MethodAndAttributeParameterTypeMismatch_ThrowsScriptException()
        ////{
        ////    var ex = Assert.Throws<ScriptException>(() => TargetCreator.CreateTargetFromMethodAttributes(new BuildScriptParameterTypeMismatch(), _taskSession));
        ////    Assert.Equal("Failed to create target 'Test'. Attribute parameter 21 does not match 'TestTarget' method parameter 21. Expected System.String Actual: System.Int32", ex.Message);
        ////}
    }
}

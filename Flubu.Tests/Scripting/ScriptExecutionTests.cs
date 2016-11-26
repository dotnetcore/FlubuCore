using DotNet.Cli.Flubu.Scripting;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Flubu.Tests.Scripting
{
    public class ScriptExecutionTests
    {
        private readonly Mock<IFileWrapper> _fileLoader = new Mock<IFileWrapper>();

        private readonly ScriptLoader _loader;

        public ScriptExecutionTests()
        {
            _loader = new ScriptLoader(_fileLoader.Object);
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadDefaultScript()
        {
            _fileLoader.Setup(i => i.ReadAllText("e.cs")).Returns(@"
using System;
using FlubuCore.Context;
using FlubuCore.Scripting;

public class MyBuildScript : DefaultBuildScript
{
    protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
    {
        System.Console.WriteLine(""2222"");
        }

        protected override void ConfigureTargets(ITaskContext context)
        {
            Console.WriteLine(""2222"");
        }
    }");

            IBuildScript t = await _loader.FindAndCreateBuildScriptInstanceAsync("e.cs");
            var provider = new ServiceCollection().BuildServiceProvider();
            t.Run(new TaskSession(
                null,
                new TargetTree(provider, new DotnetTaskFactory(provider)),
                new CommandArguments(),
                new DotnetTaskFactory(provider),
                new CoreTaskFluentInterface(new LinuxTaskFluentInterface()),
                new TaskFluentInterface(new IisTaskFluentInterface()),
                new TargetFluentInterface(),
                new TaskContextSession()));
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadSimpleScript()
        {
            _fileLoader.Setup(i => i.ReadAllText("e.cs"))
                .Returns(@"
using FlubuCore.Scripting;
using System;
using FlubuCore.Context;

public class MyBuildScript : IBuildScript
{
    public void Run(ITaskSession session)
    {
        Console.WriteLine(""11"");        
    }
}");

            IBuildScript t = await _loader.FindAndCreateBuildScriptInstanceAsync("e.cs");

            var provider = new ServiceCollection().BuildServiceProvider();
            t.Run(new TaskSession(
                null,
                new TargetTree(provider, new DotnetTaskFactory(provider)),
                new CommandArguments(),
                new DotnetTaskFactory(provider),
                new CoreTaskFluentInterface(new LinuxTaskFluentInterface()),
                new TaskFluentInterface(new IisTaskFluentInterface()),
                new TargetFluentInterface(),
                new TaskContextSession()));
        }

        [Theory]
        [InlineData("Foo\r\npublic class SomeBuildScript : Base\r\n{\r\n}", "SomeBuildScript")]
        [InlineData("Foo\r\npublic class BuildScript    : Base\r\n{\r\n}", "BuildScript")]
        [InlineData("Foo\r\npublic   class Deploy : Base\r\n{\r\n}", "Deploy")]
        [InlineData("Foo\r\npublic class _LameScript123 \r\n{\r\n}", "_LameScript123")]
        [InlineData("Foo\r\nbooo\r\npublic class BuildScript", "BuildScript")]
        public void GetClassNameFromBuildScriptCodeTest(string code, string expectedClassName)
        {
            var result = _loader.GetClassNameFromBuildScriptCode(code);
            Assert.Equal(expectedClassName, result);
        }
    }
}
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

        private readonly IScriptLoader _loader;

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
    protected override void ConfigureBuildProperties(ITaskSession session)
    {
        System.Console.WriteLine(""2222"");
        }

        protected override void ConfigureTargets(ITaskSession session)
        {
            Console.WriteLine(""2222"");
        }
    }");

            IBuildScript t = await _loader.FindAndCreateBuildScriptInstanceAsync("e.cs");
            var provider = new ServiceCollection().BuildServiceProvider();
            t.Run(new TaskSession(
                null,
                new TaskContextSession(),
                new TargetTree(provider, new DotnetTaskFactory(provider)),
                new CommandArguments(),
                new DotnetTaskFactory(provider),
                new CoreTaskFluentInterface(),
                new TaskFluentInterface(new IisTaskFluentInterface())));
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
    public int Run(ITaskSession session)
    {
        Console.WriteLine(""11"");
        return 0;
    }
}");

            IBuildScript t = await _loader.FindAndCreateBuildScriptInstanceAsync("e.cs");

            var provider = new ServiceCollection().BuildServiceProvider();
            t.Run(new TaskSession(
                null,
                new TaskContextSession(),
                new TargetTree(provider, new DotnetTaskFactory(provider)),
                new CommandArguments(),
                new DotnetTaskFactory(provider),
                new CoreTaskFluentInterface(),
                new TaskFluentInterface(new IisTaskFluentInterface())));
        }
    }
}
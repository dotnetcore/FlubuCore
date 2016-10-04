using System.Threading.Tasks;
using DotNet.Cli.Flubu.Scripting;
using FlubuCore.Context;
using FlubuCore.Infrastructure;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using Microsoft.DotNet.Cli.Utils;
using Moq;
using Xunit;

namespace Flubu.Tests.Scripting
{
    public class ScriptExecutionTests
    {
        private readonly Mock<IFileLoader> _fileLoader = new Mock<IFileLoader>();

        private readonly IScriptLoader _loader;

        public ScriptExecutionTests()
        {
            _loader = new ScriptLoader(_fileLoader.Object);
        }

        [Fact]
        public async Task LoadDefaultScript()
        {
            _fileLoader.Setup(i => i.LoadFile("e.cs")).Returns(@"
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

            t.Run(new TaskSession(null, new TaskContextSession(), new TargetTree(), new CommandArguments(), new ComponentProvider(new CommandFactory())));
        }

        [Fact]
        public async Task LoadSimpleScript()
        {
            _fileLoader.Setup(i => i.LoadFile("e.cs"))
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
            t.Run(new TaskSession(null, new TaskContextSession(), new TargetTree(), new CommandArguments(), new ComponentProvider(new CommandFactory())));
        }
    }
}
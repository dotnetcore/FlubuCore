using System.Threading.Tasks;
using Flubu.Scripting;
using Flubu.Tasks;
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

        [Fact(Skip = "buildscript not available in automatic tests")]
        public async Task LoadDefaultScript()
        {
            _fileLoader.Setup(i => i.LoadFile("e.cs")).Returns(@"
using System;
using System.Diagnostics;

public partial class MyBuildScript
{
    protected override void ConfigureBuildProperties(flubu.TaskSession session)
    {
        Console.WriteLine(""2222"");
        Debug.WriteLine(""2222"");
        }

        protected override void ConfigureTargets(Flubu.Targeting.TargetTree targetTree, Flubu.Scripting.CommandArguments args)
        {
            WriteLine(""2222"");
            Debug.WriteLine(""2222"");
        }
    }");

            IBuildScript t = await _loader.FindAndCreateBuildScriptInstance("e.cs");

            t.Run(new TaskSession(null, new TaskContextProperties(),  new CommandArguments()));
        }

        [Fact]
        public async Task LoadSimpleScript()
        {
            _fileLoader.Setup(i => i.LoadFile("e.cs"))
                .Returns(@"
using Flubu.Scripting;
using System;
using Flubu.Tasks;

public class MyBuildScript : IBuildScript
{
    public int Run(ITaskSession session)
    {
        Console.WriteLine(""11"");
        return 0;
    }
}");

            IBuildScript t = await _loader.FindAndCreateBuildScriptInstance("e.cs");
            t.Run(new TaskSession(null, new TaskContextProperties(),  new CommandArguments()));
        }
    }
}
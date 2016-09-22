using System.Threading.Tasks;
using Flubu.Scripting;
using Moq;
using Xunit;

namespace Flubu.Tests.Scripting
{
    public class ScriptExecutionTests
    {
        private readonly Mock<IFileLoader> fileLoader = new Mock<IFileLoader>();

        private readonly IScriptLoader loader;

        public ScriptExecutionTests()
        {
            loader = new ScriptLoader(fileLoader.Object);
        }

        [Fact(Skip = "buildscript not available in automatic tests")]
        public async Task LoadDefaultScript()
        {
            fileLoader.Setup(i => i.LoadFile("e.cs")).Returns(@"
using System;
using System.Diagnostics;

public partial class MyBuildScript
{
    protected override void ConfigureBuildProperties(flubu.TaskSession session)
    {
        Console.WriteLine(""2222"");
        Debug.WriteLine(""2222"");
        }

        protected override void ConfigureTargets(flubu.Targeting.TargetTree targetTree, flubu.Scripting.CommandArguments args)
        {
            WriteLine(""2222"");
            Debug.WriteLine(""2222"");
        }
    }");

            var t = await loader.FindAndCreateBuildScriptInstance("e.cs");

            t.Run(new CommandArguments());
        }

        [Fact]
        public async Task LoadSimpleScript()
        {
            fileLoader.Setup(i => i.LoadFile("e.cs"))
                .Returns(@"
using Flubu.Scripting;
using System;

public class MyBuildScript : IBuildScript
{
    public int Run(CommandArguments args)
    {
        Console.WriteLine(""11"");
        return 0;
    }
}");

            var t = await loader.FindAndCreateBuildScriptInstance("e.cs");
            t.Run(new CommandArguments());
        }
    }
}
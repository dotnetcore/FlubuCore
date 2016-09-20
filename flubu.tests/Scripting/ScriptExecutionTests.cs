using flubu.Scripting;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace flubu.tests.Scripting
{
    public class ScriptExecutionTests
    {
        private Mock<IFileLoader> _fileLoader = new Mock<IFileLoader>();
        private IScriptLoader _loader;

        public ScriptExecutionTests()
        {
            _loader = new ScriptLoader(_fileLoader.Object);
        }

        [Fact]
        public async Task LoadSimpleScript()
        {
            _fileLoader.Setup(i => i.LoadFile("e.cs")).Returns(@"public class MyBuildScript : flubu.Scripting.IBuildScript
{
    public int Run(flubu.Scripting.CommandArguments args)
    {
        System.Console.WriteLine(""11"");
        return 0;
    }
}");

            IBuildScript t = await _loader.FindAndCreateBuildScriptInstance("e.cs");
            t.Run(new CommandArguments());
        }

        [Fact]
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

        protected override void ConfigureTargets(flubu.Targeting.TargetTree targetTree, flubu.Scripting.CommandArguments args)
        {
            WriteLine(""2222"");
            Debug.WriteLine(""2222"");
        }
    }");

            IBuildScript t = await _loader.FindAndCreateBuildScriptInstance("e.cs");

            t.Run(new CommandArguments());
        }
    }

}

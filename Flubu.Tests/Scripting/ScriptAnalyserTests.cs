using DotNet.Cli.Flubu.Scripting.Analysis;
using DotNet.Cli.Flubu.Scripting.Processors;
using Xunit;

namespace Flubu.Tests.Scripting
{
    public class ScriptAnalyserTests
    {
        [Theory]
        [InlineData("Foo\r\npublic class SomeBuildScript : Base\r\n{\r\n}", "SomeBuildScript")]
        [InlineData("Foo\r\npublic class BuildScript    : Base\r\n{\r\n}", "BuildScript")]
        [InlineData("Foo\r\npublic   class Deploy : Base\r\n{\r\n}", "Deploy")]
        [InlineData("Foo\r\npublic class _LameScript123 \r\n{\r\n}", "_LameScript123")]
        [InlineData("Foo\r\nbooo\r\npublic class BuildScript", "BuildScript")]
        public void GetClassNameFromBuildScriptCodeTest(string code, string expectedClassName)
        {
            ClassDirectiveProcessor pr = new ClassDirectiveProcessor();
            AnalyserResult res = new AnalyserResult();
            pr.Process(res, code);
            Assert.Equal(expectedClassName, res.ClassName);
        }

    }
}

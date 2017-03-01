using System.Collections.Generic;
using System.Linq;
using Castle.Components.DictionaryAdapter;
using DotNet.Cli.Flubu.Scripting.Analysis;
using DotNet.Cli.Flubu.Scripting.Processor;
using DotNet.Cli.Flubu.Scripting.Processors;
using Xunit;

namespace Flubu.Tests.Scripting
{
    public class ScriptAnalyserTests
    {
        private readonly IScriptAnalyser _analyser;

        public ScriptAnalyserTests()
        {
            List<IDirectiveProcessor> processors = new List<IDirectiveProcessor>()
            {
                new ClassDirectiveProcessor(),
                new ReferenceDirectiveProcessor()
            };

            _analyser = new ScriptAnalyser(processors);
        }
    
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

        [Theory]
        [InlineData("#ref hello.dll", "hello.dll")]
        [InlineData("#ref    hello1.dll    \r\n", "hello1.dll")]
        public void ParseDll(string line, string expected)
        {
            ReferenceDirectiveProcessor pr = new ReferenceDirectiveProcessor();
            AnalyserResult res = new AnalyserResult();
            pr.Process(res, line);
            Assert.Equal(expected, res.References.First());
        }

        [Fact]
        public void Analyse()
        {
            List<string> lines = new List<string>()
            {
                "#ref",
                "//",
                "#ref hello.dll",
                "public class MyScript"
            };

            var res = _analyser.Analyze(lines);

            Assert.Equal("MyScript", res.ClassName);
            Assert.Equal(1, res.References.Count);
            Assert.Equal(2, lines.Count);
        }
    }
}

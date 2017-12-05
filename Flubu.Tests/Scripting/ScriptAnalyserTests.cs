using System.Collections.Generic;
using System.Linq;
using FlubuCore.Scripting.Analysis;
using FlubuCore.Scripting.Processors;
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
                new AssemblyDirectiveProcessor(),
                new NamespaceDirectiveProcessor(),
                new CsDirectiveProcessor()
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
            pr.Process(res, code, 1);
            Assert.Equal(expectedClassName, res.ClassName);
        }

        [Theory]
        [InlineData("//#ass c:\\hello.dll", "c:\\hello.dll")]
        [InlineData("//#ass c:\\hello1.dll    \r\n", "c:\\hello1.dll")]
        public void ParseDll(string line, string expected)
        {
            AssemblyDirectiveProcessor pr = new AssemblyDirectiveProcessor();
            AnalyserResult res = new AnalyserResult();
            pr.Process(res, line, 1);
            Assert.Equal(expected, res.References.First());
        }

        [Theory]
        [InlineData("//#imp c:\\test.cs", "c:\\test.cs")]
        [InlineData("//#imp c:\\test.cs    \r\n", "c:\\test.cs")]
        public void ParseCs(string line, string expected)
        {
            CsDirectiveProcessor pr = new CsDirectiveProcessor();
            AnalyserResult res = new AnalyserResult();
            pr.Process(res, line, 1);
            Assert.Equal(expected, res.CsFiles.First());
        }

        [Fact]
        public void Analyse()
        {
            List<string> lines = new List<string>()
            {
                "//#ass",
                "//",
                "//#ass hello.dll",
                "//#imp test.cs",
                "public class MyScript"
            };

            var res = _analyser.Analyze(lines);

            Assert.Equal("MyScript", res.ClassName);
            Assert.Single(res.References);
            Assert.Single(res.CsFiles);
            Assert.Equal(2, lines.Count);
        }

        [Fact]
        public void RemoveNamespaceAnalyse()
        {
            List<string> lines = new List<string>()
            {
                "//fsa",
                "namespace test",
                "{",
                "public class MyScript",
                "{",
                "}",
                "}"
            };

             _analyser.Analyze(lines);
            Assert.Equal(4, lines.Count);
        }
    }
}

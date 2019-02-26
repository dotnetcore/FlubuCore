using System.Collections.Generic;
using System.Linq;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting.Analysis;
using FlubuCore.Scripting.Analysis.Processors;
using FlubuCore.Scripting.Attributes;
using Moq;
using Xunit;

namespace FlubuCore.Tests.Scripting
{
    public class ScriptAnalyserTests
    {
        private readonly IScriptAnalyzer _analyzer;

        private Mock<IFileWrapper> _fileWrapper;

        private Mock<IPathWrapper> _pathWrapper;

        public ScriptAnalyserTests()
        {
            _fileWrapper = new Mock<IFileWrapper>();

            _pathWrapper = new Mock<IPathWrapper>();

            List<IScriptProcessor> processors = new List<IScriptProcessor>()
            {
                new ClassDirectiveProcessor(),
                new AssemblyDirectiveProcessor(_fileWrapper.Object, _pathWrapper.Object),
                new NamespaceProcessor(),
                new CsDirectiveProcessor(),
                new NugetPackageDirectirveProcessor()
            };

            _analyzer = new ScriptAnalyzer(processors);
        }

        [Theory]
        [InlineData("Foo\r\npublic class SomeBuildScript : Base\r\n{\r\n}", "SomeBuildScript", false)]
        [InlineData("Foo\r\npublic class BuildScript    : Base\r\n{\r\n}", "BuildScript", false)]
        [InlineData("Foo\r\npublic partial class BuildScript    : Base\r\n{\r\n}", "BuildScript", true)]
        [InlineData("Foo\r\npublic   class Deploy : Base\r\n{\r\n}", "Deploy", false)]
        [InlineData("Foo\r\npublic class _LameScript123 \r\n{\r\n}", "_LameScript123", false)]
        [InlineData("Foo\r\nbooo\r\npublic class BuildScript", "BuildScript", false)]
        [InlineData("Foo\r\npublic class BuildScriptpartial    : Base\r\n{\r\n}", "BuildScriptpartial", false)]
        public void GetClassNameFromBuildScriptCodeTest(string code, string expectedClassName, bool isPartial)
        {
            ClassDirectiveProcessor pr = new ClassDirectiveProcessor();
            ScriptAnalyzerResult res = new ScriptAnalyzerResult();
            pr.Process(res, code, 1);
            Assert.Equal(expectedClassName, res.ClassName);
            Assert.Equal(isPartial, res.IsPartial);
        }

        [Theory]
        [InlineData("//#ass c:\\hello.dll", "c:\\hello.dll")]
        [InlineData("//#ass c:\\hello1.dll    \r\n", "c:\\hello1.dll")]
        [Trait("Category", "OnlyWindows")]
        public void ParseDll(string line, string expected)
        {
            AssemblyDirectiveProcessor pr = new AssemblyDirectiveProcessor(_fileWrapper.Object, _pathWrapper.Object);
            _pathWrapper.Setup(x => x.GetExtension(expected)).Returns(".dll");
            _fileWrapper.Setup(x => x.Exists(expected)).Returns(true);
            ScriptAnalyzerResult res = new ScriptAnalyzerResult();
            pr.Process(res, line, 1);
            Assert.Equal(expected, res.AssemblyReferences.First().FullPath);
        }

        [Theory]
        [InlineData("//#nuget FlubuCore, 2.7.0", "FlubuCore", "2.7.0")]
        [InlineData("//#nuget RockStar, 2.0.0  \r\n", "RockStar", "2.0.0")]
        public void NugetReference(string line, string expectedId, string expectedVersion)
        {
            NugetPackageDirectirveProcessor pr = new NugetPackageDirectirveProcessor();
            ScriptAnalyzerResult res = new ScriptAnalyzerResult();
            pr.Process(res, line, 1);
            Assert.Equal(expectedId, res.NugetPackageReferences.First().Id);
            Assert.Equal(expectedVersion, res.NugetPackageReferences.First().Version);
        }

        [Theory]
        [InlineData("//#imp c:\\test.cs", "c:\\test.cs")]
        [InlineData("//#imp c:\\test.cs    \r\n", "c:\\test.cs")]
        [Trait("Category", "OnlyWindows")]
        public void ParseCs(string line, string expected)
        {
            CsDirectiveProcessor pr = new CsDirectiveProcessor();
            ScriptAnalyzerResult res = new ScriptAnalyzerResult();
            pr.Process(res, line, 1);
            Assert.Equal(expected, res.CsFiles.First());
        }

        [Theory]
        [InlineData("[DisableLoadScriptReferencesAutomatically]", FlubuCore.Scripting.Attributes.ScriptAttributes.DisableLoadScriptReferencesAutomatically)]
        [InlineData("[DisableLoadScriptReferencesAutomaticallyAttribute]", FlubuCore.Scripting.Attributes.ScriptAttributes.DisableLoadScriptReferencesAutomatically)]
        [InlineData("[DisableLoadScriptReferencesAutomatically2]", null)]
        [InlineData("[ADisableLoadScriptReferencesAutomatically]", null)]
        [InlineData("DisableLoadScriptReferencesAutomatically]", null)]
        [InlineData("[DisableLoadScriptReferencesAutomatically", null)]
        public void ScriptAttributes(string line, ScriptAttributes? expected)
        {
            AttributesProcessor pr = new AttributesProcessor();
            ScriptAnalyzerResult res = new ScriptAnalyzerResult();
            pr.Process(res, line, 1);
            if (expected.HasValue)
            {
                Assert.True(res.ScriptAttributes.Contains(expected.Value));
            }
            else
            {
                Assert.True(res.ScriptAttributes.Count == 0);
            }
        }

        [Fact]
        public void Analyze()
        {
            List<string> lines = new List<string>()
            {
                "//#ass",
                "//",
                "//#ass hello.dll",
                "//#nuget Package, 2.0.0",
                "//#imp test.cs",
                "public class MyScript"
            };

            _pathWrapper.Setup(x => x.GetExtension(It.IsAny<string>())).Returns(".dll");
            _fileWrapper.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);

            var res = _analyzer.Analyze(lines);

            Assert.Equal("MyScript", res.ClassName);
            Assert.Single(res.AssemblyReferences);
            Assert.Single(res.NugetPackageReferences);
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

            _analyzer.Analyze(lines);
            Assert.Equal(4, lines.Count);
        }
    }
}

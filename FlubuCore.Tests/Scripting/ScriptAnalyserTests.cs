using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting.Analysis;
using FlubuCore.Scripting.Analysis.Processors;
using FlubuCore.Scripting.Attributes;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FlubuCore.Tests.Scripting
{
    public class ScriptAnalyserTests
    {
        private readonly IScriptAnalyzer _analyzer;

        private Mock<IFileWrapper> _fileWrapper;

        private Mock<IPathWrapper> _pathWrapper;

        private Mock<IDirectoryWrapper> _directory;

        public ScriptAnalyserTests()
        {
            _fileWrapper = new Mock<IFileWrapper>();

            _pathWrapper = new Mock<IPathWrapper>();

            _directory = new Mock<IDirectoryWrapper>();

            List<IScriptProcessor> processors = new List<IScriptProcessor>()
            {
                new ClassDirectiveProcessor(),
                new AssemblyDirectiveProcessor(_fileWrapper.Object, _pathWrapper.Object, new Mock<ILogger<AssemblyDirectiveProcessor>>().Object),
                new NamespaceProcessor(),
                new CsDirectiveProcessor(),
                new NugetPackageDirectirveProcessor(new Mock<ILogger<NugetPackageDirectirveProcessor>>().Object)
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
            AssemblyDirectiveProcessor pr = new AssemblyDirectiveProcessor(_fileWrapper.Object, _pathWrapper.Object, new Mock<ILogger<AssemblyDirectiveProcessor>>().Object);
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
            NugetPackageDirectirveProcessor pr = new NugetPackageDirectirveProcessor(new Mock<ILogger<NugetPackageDirectirveProcessor>>().Object);
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
        [InlineData("[DisableLoadScriptReferencesAutomatically]", FlubuCore.Scripting.Attributes.ScriptConfigAttributes.DisableLoadScriptReferencesAutomatically)]
        [InlineData("[DisableLoadScriptReferencesAutomaticallyAttribute]", FlubuCore.Scripting.Attributes.ScriptConfigAttributes.DisableLoadScriptReferencesAutomatically)]
        [InlineData("[DisableLoadScriptReferencesAutomatically2]", null)]
        [InlineData("[ADisableLoadScriptReferencesAutomatically]", null)]
        [InlineData("DisableLoadScriptReferencesAutomatically]", null)]
        [InlineData("[DisableLoadScriptReferencesAutomatically", null)]
        [InlineData("[AlwaysRecompileScriptAttribute]", ScriptConfigAttributes.AlwaysRecompileScript)]
        [InlineData("[AlwaysRecompileScript]", ScriptConfigAttributes.AlwaysRecompileScript)]
        [InlineData("[CreateBuildScriptInstanceOldWayAttribute]", ScriptConfigAttributes.CreateBuildScriptInstanceOldWayAttribute)]
        [InlineData("[CreateBuildScriptInstanceOldWay]", ScriptConfigAttributes.CreateBuildScriptInstanceOldWayAttribute)]
        public void ScriptConfigAttributesTests(string line, ScriptConfigAttributes? expected)
        {
            AttributesProcessor pr = new AttributesProcessor(_fileWrapper.Object, _pathWrapper.Object, _directory.Object);
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

        [Theory]
        [InlineData("[Assembly($\"c:\\hello.dll\")]")]
        [InlineData("[Assembly(@@\"c:\\hello.dll\")]")]
        [InlineData("[Assembly(   $\"c:\\hello.dll\")]")]
        [InlineData("[Assembly(\"c:\\hello.dll\")]")]
        [InlineData("[Assembly  (\"c:\\hello.dll\"  )  ]")]
        [Trait("Category", "OnlyWindows")]
        public void AssemblyAttribute_Succesfull(string line)
        {
            AttributesProcessor pr = new AttributesProcessor(_fileWrapper.Object, _pathWrapper.Object, _directory.Object);
            ScriptAnalyzerResult res = new ScriptAnalyzerResult();
            _pathWrapper.Setup(x => x.GetExtension("c:\\hello.dll")).Returns(".dll");
            _fileWrapper.Setup(x => x.Exists("c:\\hello.dll")).Returns(true);
            pr.Process(res, line, 1);
            Assert.Equal("c:\\hello.dll", res.AssemblyReferences.First().FullPath);
        }

        [Theory]
        [InlineData("[AssemblyFromDirectory  (\"c:\\hello\"  )  ]")]
        [Trait("Category", "OnlyWindows")]
        public void AssemblyFromDirectoryAttribute_Succesfull(string line)
        {
            AttributesProcessor pr = new AttributesProcessor(_fileWrapper.Object, _pathWrapper.Object, _directory.Object);
            ScriptAnalyzerResult res = new ScriptAnalyzerResult();
            _directory
                .Setup(x => x.GetFiles("c:\\hello", "*.dll", SearchOption.TopDirectoryOnly))
                .Returns(new string[]
                {
                    "c:\\hello\\test.dll",
                    "c:\\hello\\test2.dll",
                });

            _pathWrapper.Setup(x => x.GetExtension("c:\\hello\\test.dll")).Returns(".dll");
            _pathWrapper.Setup(x => x.GetExtension("c:\\hello\\test2.dll")).Returns(".dll");
            _fileWrapper.Setup(x => x.Exists("c:\\hello\\test.dll")).Returns(true);
            _fileWrapper.Setup(x => x.Exists("c:\\hello\\test2.dll")).Returns(true);
            pr.Process(res, line, 1);
            Assert.Equal("c:\\hello\\test.dll", res.AssemblyReferences.First().FullPath);
            Assert.Equal("c:\\hello\\test2.dll", res.AssemblyReferences[1].FullPath);

            _fileWrapper.VerifyAll();
            _pathWrapper.VerifyAll();
            _directory.VerifyAll();
        }

        [Theory]
        [InlineData("[Include($\".\\Test.cs\")]")]
        [Trait("Category", "OnlyWindows")]
        public void IncludeAttribute_Succesfull(string line)
        {
            AttributesProcessor pr = new AttributesProcessor(_fileWrapper.Object, _pathWrapper.Object, _directory.Object);
            ScriptAnalyzerResult res = new ScriptAnalyzerResult();
            pr.Process(res, line, 1);
            Assert.Contains("\\Test.cs", res.CsFiles[0]);
        }

        [Theory]
        [InlineData("namespace FlubuCore.Helpers   ")]
        [InlineData("namespace   FlubuCore.Helpers")]
        public void NamespaceProcessor_Succesfull(string line)
        {
            NamespaceProcessor pr = new NamespaceProcessor();
            ScriptAnalyzerResult res = new ScriptAnalyzerResult();
            pr.Process(res, line, 2);
            Assert.Equal(2, res.NamespaceIndex);
            Assert.Equal("FlubuCore.Helpers", res.Namespace);
        }

        [Theory]
        [InlineData("[IncludeFromDirectory($\".\\Test\", true)]", true)]
        [InlineData("[IncludeFromDirectory($\".\\Test\")]", false)]
        [Trait("Category", "OnlyWindows")]
        public void IncludeFromDirectoryAttribute_Succesfull(string line, bool expectedIncludeSubDir)
        {
            AttributesProcessor pr = new AttributesProcessor(_fileWrapper.Object, _pathWrapper.Object, _directory.Object);
            ScriptAnalyzerResult res = new ScriptAnalyzerResult();
            pr.Process(res, line, 1);
            Assert.Contains("\\Test", res.CsDirectories[0].Item1.path);
            Assert.Equal(expectedIncludeSubDir, res.CsDirectories[0].Item1.includeSubDirectories);
        }

        [Theory]
        [InlineData(@"[NugetPackage($ ""FlubuCore"",   ""2.7.0"")]")]
        [InlineData(@"[NugetPackage(@@ ""FlubuCore"",   ""2.7.0"")]")]
        [InlineData(@"[NugetPackage  (@@ ""FlubuCore"",""2.7.0""  )   ]")]
        [InlineData(@"[NugetPackage(""FlubuCore"",""2.7.0"")]")]
        [InlineData(@"[NugetPackage($""FlubuCore"",""2.7.0"")]")]
        public void NugetPackageAttribute_Succesfull(string line)
        {
            AttributesProcessor pr = new AttributesProcessor(_fileWrapper.Object, _pathWrapper.Object, _directory.Object);
            ScriptAnalyzerResult res = new ScriptAnalyzerResult();
            pr.Process(res, line, 1);
            Assert.Equal("FlubuCore", res.NugetPackageReferences.First().Id);
            Assert.Equal("2.7.0", res.NugetPackageReferences.First().Version);
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

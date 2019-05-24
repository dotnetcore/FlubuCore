using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting.Analysis;
using Xunit;

namespace FlubuCore.Tests.Scripting
{
    public class ProjectFileAnalyzerTests
    {
        private IProjectFileAnalyzer _analyzer;

        public ProjectFileAnalyzerTests()
        {
            _analyzer = new ProjectFileAnalyzer(new FileWrapper());
        }

        [Fact]
        public void Analyze_WithNugetAndAssemblyRefrences_Succesfull()
        {
            var result = _analyzer.Analyze("TestData/ProjectFiles/Build.csproj");
            Assert.True(result.ProjectFileFound);
            Assert.Equal(2, result.NugetReferences.Count);
            Assert.Equal("LiteDB", result.NugetReferences[0].Id);
            Assert.Equal("4.1.2", result.NugetReferences[0].Version);
            Assert.Equal("Moq", result.NugetReferences[1].Id);
            Assert.Equal("8.0.0", result.NugetReferences[1].Version);
            Assert.Equal(3, result.AssemblyReferences.Count);
            Assert.Equal("Newtonsoft.Json", result.AssemblyReferences[0].Name);
            Assert.Contains(@"..\packages\Newtonsoft.Json.10.0.2\lib\net45\Newtonsoft.Json.dll", result.AssemblyReferences[0].Path);
            Assert.StartsWith("TestData", result.AssemblyReferences[0].Path);
            Assert.Equal("System.Net.Http", result.AssemblyReferences[2].Name);
            Assert.Null(result.AssemblyReferences[2].Path);
        }
    }
}

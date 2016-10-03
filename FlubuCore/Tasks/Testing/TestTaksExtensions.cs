using System.IO;
using FlubuCore.Targeting;
using FlubuCore.Tasks.NetCore;

namespace FlubuCore.Tasks.Testing
{
    public static class TestTaksExtensions
    {
        public static ITarget UnitTest(this ITarget target, params string[] projects)
        {
            foreach (string project in projects)
            {
                target.AddTask(Dotnet.Test(project));
            }

            return target;
        }

        public static ITarget CoverageDotnet(this ITarget target, params string[] projects)
        {
            foreach (string project in projects)
            {
                target.AddTask(CoverageDotnet(project, null, null));
            }

            return target;
        }

        public static ITarget CoverageDotnet(this ITarget target, string projectPath, string output, params string[] excludeList)
        {
            target.AddTask(CoverageDotnet(projectPath, output, excludeList));
            return target;
        }

        public static OpenCoverTask CoverageDotnet(string projectPath, string output, params string[] excludeList)
        {
            if (string.IsNullOrEmpty(output))
            {
                output = $"{Path.GetFileNameWithoutExtension(projectPath)}cover.xml";
            }

            return new OpenCoverTask()
                .TestExecutableArgs($"test {projectPath}")
                .Output(output)
                .UseDotNet()
                .AddExclude(excludeList);
        }

        public static OpenCoverToCoberturaTask ConvertToCobertura(string input, string output)
        {
            return new OpenCoverToCoberturaTask(input, output);
        }

        public static ITarget ConvertToCobertura(this ITarget target, string input, string output)
        {
            target.AddTask(ConvertToCobertura(input, output));
            return target;
        }

        public static CoverageReportTask CoverageReport(string targetDir, params string[] inputFiles)
        {
            return new CoverageReportTask(inputFiles)
                .TargetFolder(targetDir);
        }

        public static ITarget CoverageReport(this ITarget target, params string[] inputFiles)
        {
            target.AddTask(CoverageReport("coverage", inputFiles));
            return target;
        }
    }
}

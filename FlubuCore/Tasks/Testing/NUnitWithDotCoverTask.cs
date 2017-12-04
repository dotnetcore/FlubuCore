using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FlubuCore.Context;
using FlubuCore.IO;
using FlubuCore.Tasks.Nuget;
using FlubuCore.Tasks.Process;
using FlubuCore.Tasks.Text;

namespace FlubuCore.Tasks.Testing
{
    /// <summary>
    /// Runs NUnit tests in combination with dotCover test coverage analysis.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The task uses dotCover command line tool to run NUnit command line runner
    /// which executes tests for the specified assembly or C# project.
    /// </para>
    /// <para>
    /// The task uses <see cref="DownloadNugetPackageInUserRepositoryTask"/> to download dotCover command
    /// line tool into the running user's local application data directory. If the tool is already there,
    /// the task skips downloading it.
    /// </para>
    /// </remarks>
    public class NUnitWithDotCoverTask : TaskBase<int, NUnitWithDotCoverTask>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitWithDotCoverTask"/> class that
        /// will execute tests in the specified <see cref="testAssemblyFileNames"/> list of test assemblies using 
        /// the specified NUnit test runner executable.
        /// </summary>
        /// <param name="nunitRunnerFileName">The file path to NUnit's console runner.</param>
        /// <param name="testAssemblyFileNames">The list of of file paths to the assemblies containing unit tests.</param>
        public NUnitWithDotCoverTask(string nunitRunnerFileName, params string[] testAssemblyFileNames)
        {
            if (string.IsNullOrEmpty(nunitRunnerFileName))
                throw new ArgumentException("NUnit Runner file name should not be null or empty string", "nunitRunnerFileName");

            this.nunitRunnerFileName = nunitRunnerFileName;
            this.testAssemblyFileNames = testAssemblyFileNames;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes NUnit unit tests with dot cover on assemblies: {testAssemblyFileNames.Concat(x => x, ", ")}";
                }

                return _description;
            }
            set { _description = value; }
        }

        /// <summary>
        /// If Set build will fail if the test coverage of any
        /// class is below <see cref="MinRequiredCoverage"/>. 
        /// </summary>
        /// <remarks>
        /// If <see cref="FailBuildOnViolations"/>
        /// is not set the task will only print out information about violating classes
        /// without failing the build.
        /// The default value is <c>true</c>.
        /// </remarks>
        public NUnitWithDotCoverTask FailBuildOnViolations()
        {
            failBuildOnViolations = true;
            return this;
        }

        /// <summary>
        /// Sets the minimum required test coverage percentage. 
        /// If any class has the test coverage below this value and <see cref="FailBuildOnViolations"/>
        /// is set to <c>true</c>, the task will fail the build. 
        /// </summary>
        /// <remarks>
        /// The default value is 75%.
        /// </remarks>
        public NUnitWithDotCoverTask MinRequiredCoverage(int minRequiredCoverage)
        {
            this.minRequiredCoverage = minRequiredCoverage;
            return this;
        }

        /// <summary>
        /// Sets the command line options for NUnit console runner (as a single string).
        /// </summary>
        /// <remarks>
        /// The default options are <c>/labels /nodots</c>.
        /// </remarks>
        public NUnitWithDotCoverTask NUnitCmdLineOptions(string nunitCmdLineOptions)
        {
            this.nunitCmdLineOptions = nunitCmdLineOptions;
            return this;
        }

        /// <summary>
        /// Sets the dotCover filters that will be passed to dotCover's <c>/Filters</c> command line parameter.
        /// </summary>
        /// <remarks>
        /// The default filters are set to <c>-:module=*.Tests;-:class=*Contract;-:class=*Contract`*</c>.
        /// For more information, visit <a href="https://www.jetbrains.com/dotcover/help/dotCover__Console_Runner_Commands.html">here</a>.
        /// </remarks>
        /// <seealso cref="DotCoverAttributeFilters"/>
        public NUnitWithDotCoverTask DotCoverFilters(string dotCoverFilters)
        {
            this.dotCoverFilters = dotCoverFilters;
            return this;
        }

        /// <summary>
        /// Sets the dotCover attribute filters that will be passed to dotCover's <c>/AttributeFilters</c> command line parameter.
        /// Attribute filters tell dotCover to skip the analysis of any code that has the specified attribute(s) applied.
        /// </summary>
        /// <remarks>
        /// The default attribute filters are set to <c>"*.ExcludeFromCodeCoverageAttribute"</c>.
        /// For more information, visit <a href="https://www.jetbrains.com/dotcover/help/dotCover__Console_Runner_Commands.html">here</a>.
        /// </remarks>
        /// <seealso cref="DotCoverFilters"/>
        public NUnitWithDotCoverTask DotCoverAttributeFilters(string dotCoverAttributeFilters)
        {
            this.dotCoverAttributeFilters = dotCoverAttributeFilters;
            return this;
        }

        /// <summary>
        /// Gets the path to the generated dotCover test coverage XML report.
        /// </summary>
        /// <seealso cref="CoverageHtmlReportFileName"/>
        public string CoverageXmlReportFileName
        {
            get { return coverageXmlReportFileName; }
        }

        /// <summary>
        /// Gets the path to the generated dotCover test coverage HTML report.
        /// </summary>
        /// <seealso cref="CoverageXmlReportFileName"/>
        public string CoverageHtmlReportFileName
        {
            get { return coverageHtmlReportFileName; }
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            string dotCoverExeFileName;
            if (!EnsureDotCoverIsAvailable(context, out dotCoverExeFileName))
                return -1;

            List<string> snapshots = new List<string>();
            foreach (string testAssemblyFileName in testAssemblyFileNames)
            {
                string snapshotFileName = RunTestsForAssembly(context, testAssemblyFileName, dotCoverExeFileName);
                snapshots.Add(snapshotFileName);
            }

            string finalSnapshotFileName;
            if (snapshots.Count > 1)
                finalSnapshotFileName = MergeCoverageSnapshots(context, dotCoverExeFileName, snapshots);
            else
                finalSnapshotFileName = snapshots[0];

            coverageXmlReportFileName = GenerateCoverageReport(context, dotCoverExeFileName, finalSnapshotFileName, "XML");
            coverageHtmlReportFileName = GenerateCoverageReport(context, dotCoverExeFileName, finalSnapshotFileName, "HTML");

            AnalyzeCoverageResults(context);

            return 0;
        }

        private static bool EnsureDotCoverIsAvailable(ITaskContext context, out string dotCoverExeFileName)
        {
            const string DotCoverCmdLineToolsPackageId = "JetBrains.dotCover.CommandLineTools";

            DownloadNugetPackageInUserRepositoryTask downloadPackageTask =
                new DownloadNugetPackageInUserRepositoryTask(DotCoverCmdLineToolsPackageId);
            downloadPackageTask.Execute(context);

            dotCoverExeFileName = Path.Combine(downloadPackageTask.PackageDirectory, "tools/dotCover.exe");

            if (!File.Exists(dotCoverExeFileName))
            {
                context.LogError(
                    $"R# dotCover is not present in the expected location ('{dotCoverExeFileName}'), cannot run test coverage analysis");

                return false;
            }

            return true;
        }

        private string RunTestsForAssembly(ITaskContext context, string testAssemblyFileName, string dotCoverExeFileName)
        {
            string assemblyId;
            FileFullPath assemblyFullFileName = ExtractFullAssemblyFileName(testAssemblyFileName, out assemblyId);

            string buildDir = context.Properties[BuildProps.BuildDir];
            string snapshotFileName = Path.Combine(buildDir, "{0}-coverage.dcvr".Fmt(assemblyId));

            RunCoverTask(context, assemblyFullFileName, dotCoverExeFileName, snapshotFileName);

            return snapshotFileName;
        }

        private void RunCoverTask(
            ITaskContext context,
            IPathBuilder assemblyFullFileName,
            string dotCoverExeFileName,
            string snapshotFileName)
        {
            string projectDir = Path.GetDirectoryName(assemblyFullFileName.ToString());
            string projectBinFileName = Path.GetFileName(assemblyFullFileName.FileName);

            context.LogInfo("Running unit tests (with code coverage)...");
            IRunProgramTask runDotCovertask = context.Tasks().RunProgramTask(dotCoverExeFileName);
            runDotCovertask.WithArguments("cover")
                .WithArguments("/TargetExecutable={0}", nunitRunnerFileName)
                .WithArguments("/TargetArguments={0} {1}", projectBinFileName, nunitCmdLineOptions)
                .WithArguments("/TargetWorkingDir={0}", projectDir)
                .WithArguments("/Filters={0}", dotCoverFilters)
                .WithArguments("/AttributeFilters={0}", dotCoverAttributeFilters)
                .WithArguments("/Output={0}", snapshotFileName)
                //.AddArgument("/LogFile={0}", Path.Combine(buildDir, "dotCover-log.xml"))
                .WithArguments("/ReturnTargetExitCode");
            runDotCovertask.Execute(context);
        }

        private static string MergeCoverageSnapshots(ITaskContext context, string dotCoverExeFileName, List<string> snapshots)
        {
            context.LogInfo("Merging coverage snapshots...");

            string buildDir = context.Properties[BuildProps.BuildDir];
            string mergedSnapshotFileName = Path.Combine(buildDir, "{0}.dcvr".Fmt(context.Properties[BuildProps.ProductId]));
            IRunProgramTask runDotCovertask = context.Tasks().RunProgramTask(dotCoverExeFileName);
            runDotCovertask
                .WithArguments("merge")
                .WithArguments("/Source={0}", snapshots.Concat(x => x, ";"))
                .WithArguments("/Output={0}", mergedSnapshotFileName)

                //.AddArgument("/LogFile={0}", Path.Combine(buildDir, "dotCover-log.xml"))
                ;
            runDotCovertask.Execute(context);
            return mergedSnapshotFileName;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        private static string GenerateCoverageReport(
            ITaskContext context,
            string dotCoverExeFileName,
            string snapshotFileName,
            string reportType)
        {
            context.LogInfo($"Generating code coverage {reportType} report...");

            string buildDir = context.Properties[BuildProps.BuildDir];

            string coverageReportFileName = Path.Combine(buildDir, "dotCover-results.{0}".Fmt(reportType.ToLowerInvariant()));
            IRunProgramTask runDotCovertask = context.Tasks().RunProgramTask(dotCoverExeFileName);
            runDotCovertask
                .WithArguments("report")
                .WithArguments("/Source={0}", snapshotFileName)
                .WithArguments("/Output={0}", coverageReportFileName)
                .WithArguments("/ReportType={0}", reportType)

                //.AddArgument("/LogFile={0}", Path.Combine(buildDir, "dotCover-log.xml"))
                ;
            runDotCovertask.Execute(context);
            return coverageReportFileName;
        }

        private static FileFullPath ExtractFullAssemblyFileName(
            string testAssemblyFileName,
            out string assemblyId)
        {
            FileFullPath assemblyFullFileName = new FileFullPath(testAssemblyFileName);
            assemblyId = Path.GetFileNameWithoutExtension(assemblyFullFileName.FileName);

            return assemblyFullFileName;
        }

        private void AnalyzeCoverageResults(ITaskContext context)
        {
            const string PropertyTotalCoverage = "TotalTestCoverage";
            const string PropertyClassesWithPoorCoverageCount = "PoorCoverageCount";

            string totalCoverageExpression = "sum(/Root/Assembly[1]/@CoveragePercent)";
            string classesWithPoorCoverageExpression = string.Format(
                CultureInfo.InvariantCulture,
                "count(/Root/Assembly/Namespace/Type[@CoveragePercent<{0}])",
                minRequiredCoverage);

            EvaluateXmlTask countViolationsTask =
                new EvaluateXmlTask(coverageXmlReportFileName)
                    .AddExpression(
                        PropertyClassesWithPoorCoverageCount,
                        classesWithPoorCoverageExpression)
                    .AddExpression(
                        PropertyTotalCoverage,
                        totalCoverageExpression);
            countViolationsTask.Execute(context);

            int? totalCoverage = GetCoverageProperyValue(context, PropertyTotalCoverage);
            context.LogInfo($"Total test coverage is {totalCoverage}%");

            int? duplicatesCount = GetCoverageProperyValue(context, PropertyClassesWithPoorCoverageCount);
            if (duplicatesCount.HasValue && duplicatesCount > 0)
                FailBuildAndPrintOutCoverageReport(context, duplicatesCount);
        }

        private static int? GetCoverageProperyValue(ITaskContext context, string propertyName)
        {
            string valueStr = context.Properties[propertyName];
            if (valueStr == null)
                return null;

            return int.Parse(valueStr, CultureInfo.InvariantCulture);
        }

        private void FailBuildAndPrintOutCoverageReport(ITaskContext context, int? duplicatesCount)
        {
            context.LogInfo($"There are {duplicatesCount} classes that have the test coverage below the minimum {minRequiredCoverage}% threshold");

            string classesWithPoorCoverageExpression = string.Format(
                CultureInfo.InvariantCulture,
                "/Root/Assembly/Namespace/Type[@CoveragePercent<{0}]",
                minRequiredCoverage);

            VisitXmlFileTask findViolationsTask = new VisitXmlFileTask(coverageXmlReportFileName);

            List<Tuple<string, int>> poorCoverageClasses = new List<Tuple<string, int>>();

            findViolationsTask.AddVisitor(
                classesWithPoorCoverageExpression,
                node =>
                {
                    if (node.Attributes == null || node.ParentNode == null || node.ParentNode.Attributes == null)
                        return true;

                    string className = node.Attributes["Name"].Value;
                    string nspace = node.ParentNode.Attributes["Name"].Value;
                    int coverage = int.Parse(node.Attributes["CoveragePercent"].Value, CultureInfo.InvariantCulture);

                    poorCoverageClasses.Add(new Tuple<string, int>(nspace + "." + className, coverage));
                    return true;
                });
            findViolationsTask.Execute(context);

            poorCoverageClasses.Sort(ClassCoverageComparer);
            foreach (Tuple<string, int> tuple in poorCoverageClasses)
                context.LogInfo($"{tuple.Item1} ({tuple.Item2}%)");

            if (failBuildOnViolations)
                context.LogError("Failing the build because of poor test coverage");
        }

        private static int ClassCoverageComparer(Tuple<string, int> a, Tuple<string, int> b)
        {
            int c = a.Item2.CompareTo(b.Item2);
            if (c != 0)
                return c;

            return string.Compare(a.Item1, b.Item1, StringComparison.Ordinal);
        }

        private readonly string nunitRunnerFileName;
        private readonly IList<string> testAssemblyFileNames;
        private int minRequiredCoverage = 75;
        private string coverageXmlReportFileName;
        private string coverageHtmlReportFileName;
        private string dotCoverFilters = "-:module=*.Tests;-:class=*Contract;-:class=*Contract`*";
        private string dotCoverAttributeFilters = "*.ExcludeFromCodeCoverageAttribute";
        private string nunitCmdLineOptions = "/labels /nodots";
        private bool failBuildOnViolations = true;
        private string _description;
    }
}
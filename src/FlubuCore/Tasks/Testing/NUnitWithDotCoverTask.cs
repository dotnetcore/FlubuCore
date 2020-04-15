using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Infrastructure;
using FlubuCore.IO;
using FlubuCore.Tasks.Nuget;
using FlubuCore.Tasks.Text;

namespace FlubuCore.Tasks.Testing
{
    /// <summary>
    ///     Runs NUnit tests in combination with dotCover test coverage analysis.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The task uses dotCover command line tool to run NUnit command line runner
    ///         which executes tests for the specified assembly or C# project.
    ///     </para>
    ///     <para>
    ///         The task uses <see cref="DownloadNugetPackageInUserRepositoryTask" /> to download dotCover command
    ///         line tool into the running user's local application data directory. If the tool is already there,
    ///         the task skips downloading it.
    ///     </para>
    /// </remarks>
    public class NUnitWithDotCoverTask : TaskBase<int, NUnitWithDotCoverTask>
    {
        private readonly string _nunitRunnerFileName;
        private readonly IList<string> _testAssemblyFileNames;
        private string _description;
        private string _dotCoverAttributeFilters = "*.ExcludeFromCodeCoverageAttribute";
        private string _dotCoverFilters = "-:module=*.Tests;-:class=*Contract;-:class=*Contract`*";
        private bool _failBuildOnViolations = true;
        private int _minRequiredCoverage = 75;
        private string _nunitCmdLineOptions = "/labels /nodots";

        /// <summary>
        ///     Initializes a new instance of the <see cref="NUnitWithDotCoverTask" /> class that
        ///     will execute tests in the specified <see cref="_testAssemblyFileNames" /> list of test assemblies using
        ///     the specified NUnit test runner executable.
        /// </summary>
        /// <param name="nunitRunnerFileName">The file path to NUnit's console runner.</param>
        /// <param name="testAssemblyFileNames">The list of of file paths to the assemblies containing unit tests.</param>
        public NUnitWithDotCoverTask(string nunitRunnerFileName, params string[] testAssemblyFileNames)
        {
            if (string.IsNullOrEmpty(nunitRunnerFileName))
            {
                throw new ArgumentException("NUnit Runner file name should not be null or empty string",
                    nameof(nunitRunnerFileName));
            }

            _nunitRunnerFileName = nunitRunnerFileName;
            _testAssemblyFileNames = testAssemblyFileNames;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return
                        $"Executes NUnit unit tests with dot cover on assemblies: {_testAssemblyFileNames.Concat(x => x, ", ")}";
                }

                return _description;
            }

            set => _description = value;
        }

        /// <summary>
        ///     Gets the path to the generated dotCover test coverage XML report.
        /// </summary>
        /// <seealso cref="CoverageHtmlReportFileName" />
        public string CoverageXmlReportFileName { get; private set; }

        /// <summary>
        ///     Gets the path to the generated dotCover test coverage HTML report.
        /// </summary>
        /// <seealso cref="CoverageXmlReportFileName" />
        public string CoverageHtmlReportFileName { get; private set; }

        /// <summary>
        ///     If Set build will fail if the test coverage of any
        ///     class is below <see cref="MinRequiredCoverage" />.
        /// </summary>
        /// <remarks>
        ///     If <see cref="FailBuildOnViolations" />
        ///     is not set the task will only print out information about violating classes
        ///     without failing the build.
        ///     The default value is <c>true</c>.
        /// </remarks>
        public NUnitWithDotCoverTask FailBuildOnViolations()
        {
            _failBuildOnViolations = true;
            return this;
        }

        /// <summary>
        ///     Sets the minimum required test coverage percentage.
        ///     If any class has the test coverage below this value and <see cref="FailBuildOnViolations" />
        ///     is set to <c>true</c>, the task will fail the build.
        /// </summary>
        /// <remarks>
        ///     The default value is 75%.
        /// </remarks>
        public NUnitWithDotCoverTask MinRequiredCoverage(int minRequiredCoverage)
        {
            _minRequiredCoverage = minRequiredCoverage;
            return this;
        }

        /// <summary>
        ///     Sets the command line options for NUnit console runner (as a single string).
        /// </summary>
        /// <remarks>
        ///     The default options are <c>/labels /nodots</c>.
        /// </remarks>
        public NUnitWithDotCoverTask NUnitCmdLineOptions(string nunitCmdLineOptions)
        {
            _nunitCmdLineOptions = nunitCmdLineOptions;
            return this;
        }

        /// <summary>
        ///     Sets the dotCover filters that will be passed to dotCover's <c>/Filters</c> command line parameter.
        /// </summary>
        /// <remarks>
        ///     The default filters are set to <c>-:module=*.Tests;-:class=*Contract;-:class=*Contract`*</c>.
        ///     For more information, visit
        ///     <a href="https://www.jetbrains.com/dotcover/help/dotCover__Console_Runner_Commands.html">here</a>.
        /// </remarks>
        /// <seealso cref="DotCoverAttributeFilters" />
        public NUnitWithDotCoverTask DotCoverFilters(string dotCoverFilters)
        {
            _dotCoverFilters = dotCoverFilters;
            return this;
        }

        /// <summary>
        ///     Sets the dotCover attribute filters that will be passed to dotCover's <c>/AttributeFilters</c> command line
        ///     parameter.
        ///     Attribute filters tell dotCover to skip the analysis of any code that has the specified attribute(s) applied.
        /// </summary>
        /// <remarks>
        ///     The default attribute filters are set to <c>"*.ExcludeFromCodeCoverageAttribute"</c>.
        ///     For more information, visit
        ///     <a href="https://www.jetbrains.com/dotcover/help/dotCover__Console_Runner_Commands.html">here</a>.
        /// </remarks>
        /// <seealso cref="DotCoverFilters" />
        public NUnitWithDotCoverTask DotCoverAttributeFilters(string dotCoverAttributeFilters)
        {
            _dotCoverAttributeFilters = dotCoverAttributeFilters;
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            if (!EnsureDotCoverIsAvailable(context, out var dotCoverExeFileName))
                return -1;

            var snapshots = new List<string>();
            foreach (var testAssemblyFileName in _testAssemblyFileNames)
            {
                var snapshotFileName = RunTestsForAssembly(context, testAssemblyFileName, dotCoverExeFileName);
                snapshots.Add(snapshotFileName);
            }

            var finalSnapshotFileName = snapshots.Count > 1 ? MergeCoverageSnapshots(context, dotCoverExeFileName, snapshots) : snapshots[0];

            CoverageXmlReportFileName =
                GenerateCoverageReport(context, dotCoverExeFileName, finalSnapshotFileName, "XML");
            CoverageHtmlReportFileName =
                GenerateCoverageReport(context, dotCoverExeFileName, finalSnapshotFileName, "HTML");

            AnalyzeCoverageResults(context);

            return 0;
        }

        private static bool EnsureDotCoverIsAvailable(ITaskContext context, out string dotCoverExeFileName)
        {
            const string dotCoverCmdLineToolsPackageId = "JetBrains.dotCover.CommandLineTools";

            var downloadPackageTask =
                new DownloadNugetPackageInUserRepositoryTask(dotCoverCmdLineToolsPackageId);
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

        private string MergeCoverageSnapshots(ITaskContext context, string dotCoverExeFileName,
            List<string> snapshots)
        {
            DoLogInfo("Merging coverage snapshots...");

            var buildDir = context.Properties[DotNetBuildProps.BuildDir];
            var mergedSnapshotFileName =
                Path.Combine(buildDir, "{0}.dcvr".Fmt(context.Properties[BuildProps.ProductId]));
            var runDotCovertask = context.Tasks().RunProgramTask(dotCoverExeFileName);
            runDotCovertask
                .WithArguments("merge")
                .WithArguments("/Source={0}", snapshots.Concat(x => x, ";"))
                .WithArguments("/Output={0}", mergedSnapshotFileName)

                //.AddArgument("/LogFile={0}", Path.Combine(buildDir, "dotCover-log.xml"))
                ;
            runDotCovertask.Execute(context);
            return mergedSnapshotFileName;
        }

        private string GenerateCoverageReport(
            ITaskContext context,
            string dotCoverExeFileName,
            string snapshotFileName,
            string reportType)
        {
            DoLogInfo($"Generating code coverage {reportType} report...");

            var buildDir = context.Properties[DotNetBuildProps.BuildDir];

            var coverageReportFileName =
                Path.Combine(buildDir, "dotCover-results.{0}".Fmt(reportType.ToLowerInvariant()));
            var runDotCovertask = context.Tasks().RunProgramTask(dotCoverExeFileName);
            runDotCovertask
                .WithArguments("report")
                .WithArguments("/Source={0}", snapshotFileName)
                .WithArguments("/Output={0}", coverageReportFileName)
                .WithArguments("/ReportType={0}", reportType);

            runDotCovertask.Execute(context);
            return coverageReportFileName;
        }

        private FileFullPath ExtractFullAssemblyFileName(
            string testAssemblyFileName,
            out string assemblyId)
        {
            var assemblyFullFileName = new FileFullPath(testAssemblyFileName);
            assemblyId = Path.GetFileNameWithoutExtension(assemblyFullFileName.FileName);

            return assemblyFullFileName;
        }

        private int? GetCoverageProperyValue(ITaskContext context, string propertyName)
        {
            var valueStr = context.Properties[propertyName];
            if (valueStr == null)
                return null;

            return int.Parse(valueStr, CultureInfo.InvariantCulture);
        }

        private int ClassCoverageComparer(Tuple<string, int> a, Tuple<string, int> b)
        {
            var c = a.Item2.CompareTo(b.Item2);
            if (c != 0)
                return c;

            return string.Compare(a.Item1, b.Item1, StringComparison.Ordinal);
        }

        private string RunTestsForAssembly(ITaskContext context, string testAssemblyFileName,
            string dotCoverExeFileName)
        {
            var assemblyFullFileName = ExtractFullAssemblyFileName(testAssemblyFileName, out var assemblyId);

            var buildDir = context.Properties[DotNetBuildProps.BuildDir];
            var snapshotFileName = Path.Combine(buildDir, "{0}-coverage.dcvr".Fmt(assemblyId));

            RunCoverTask(context, assemblyFullFileName, dotCoverExeFileName, snapshotFileName);

            return snapshotFileName;
        }

        private void RunCoverTask(
            ITaskContext context,
            IPathBuilder assemblyFullFileName,
            string dotCoverExeFileName,
            string snapshotFileName)
        {
            var projectDir = Path.GetDirectoryName(assemblyFullFileName.ToString());
            var projectBinFileName = Path.GetFileName(assemblyFullFileName.FileName);

            DoLogInfo("Running unit tests (with code coverage)...");
            var runDotCovertask = context.Tasks().RunProgramTask(dotCoverExeFileName);
            runDotCovertask.WithArguments("cover")
                .WithArguments("/TargetExecutable={0}", _nunitRunnerFileName)
                .WithArguments("/TargetArguments={0} {1}", projectBinFileName, _nunitCmdLineOptions)
                .WithArguments("/TargetWorkingDir={0}", projectDir)
                .WithArguments("/Filters={0}", _dotCoverFilters)
                .WithArguments("/AttributeFilters={0}", _dotCoverAttributeFilters)
                .WithArguments("/Output={0}", snapshotFileName)
                .WithArguments("/ReturnTargetExitCode");
            runDotCovertask.Execute(context);
        }

        private void AnalyzeCoverageResults(ITaskContext context)
        {
            const string propertyTotalCoverage = "TotalTestCoverage";
            const string propertyClassesWithPoorCoverageCount = "PoorCoverageCount";

            var totalCoverageExpression = "sum(/Root/Assembly[1]/@CoveragePercent)";
            var classesWithPoorCoverageExpression = string.Format(
                CultureInfo.InvariantCulture,
                "count(/Root/Assembly/Namespace/Type[@CoveragePercent<{0}])",
                _minRequiredCoverage);

            var countViolationsTask =
                new EvaluateXmlTask(CoverageXmlReportFileName)
                    .AddExpression(
                        propertyClassesWithPoorCoverageCount,
                        classesWithPoorCoverageExpression)
                    .AddExpression(
                        propertyTotalCoverage,
                        totalCoverageExpression);
            countViolationsTask.Execute(context);

            var totalCoverage = GetCoverageProperyValue(context, propertyTotalCoverage);
            DoLogInfo($"Total test coverage is {totalCoverage}%");

            var duplicatesCount = GetCoverageProperyValue(context, propertyClassesWithPoorCoverageCount);
            if (duplicatesCount.HasValue && duplicatesCount > 0)
                FailBuildAndPrintOutCoverageReport(context, duplicatesCount);
        }

        private void FailBuildAndPrintOutCoverageReport(ITaskContext context, int? duplicatesCount)
        {
            DoLogInfo(
                $"There are {duplicatesCount} classes that have the test coverage below the minimum {_minRequiredCoverage}% threshold");

            var classesWithPoorCoverageExpression = string.Format(
                CultureInfo.InvariantCulture,
                "/Root/Assembly/Namespace/Type[@CoveragePercent<{0}]",
                _minRequiredCoverage);

            var findViolationsTask = new VisitXmlFileTask(CoverageXmlReportFileName);

            var poorCoverageClasses = new List<Tuple<string, int>>();

            findViolationsTask.AddVisitor(
                classesWithPoorCoverageExpression,
                node =>
                {
                    if (node.Attributes == null || node.ParentNode?.Attributes == null)
                        return true;

                    var className = node.Attributes["Name"].Value;
                    var nspace = node.ParentNode.Attributes["Name"].Value;
                    var coverage = int.Parse(node.Attributes["CoveragePercent"].Value, CultureInfo.InvariantCulture);

                    poorCoverageClasses.Add(new Tuple<string, int>(nspace + "." + className, coverage));
                    return true;
                });
            findViolationsTask.Execute(context);

            poorCoverageClasses.Sort(ClassCoverageComparer);
            foreach (var tuple in poorCoverageClasses)
                DoLogInfo($"{tuple.Item1} ({tuple.Item2}%)");

            if (_failBuildOnViolations)
                context.LogError("Failing the build because of poor test coverage");
        }
    }
}
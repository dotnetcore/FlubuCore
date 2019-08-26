using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.NetCore
{
    public class DotnetMsBuildTask : ExecuteDotnetTaskBase<DotnetMsBuildTask>
    {
        public DotnetMsBuildTask()
            : base(StandardDotnetCommands.MsBuild)
        {
        }

        protected override string Description { get; set; }

        /// <summary>
        /// Build these targets in this project. Use a semicolon or a comma to separate multiple targets
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        [ArgKey("-target")]
        public DotnetMsBuildTask Target(string target)
        {
            WithArgumentsKeyFromAttribute(target, separator: ":");
            return this;
        }

        /// <summary>
        /// Set or override these project-level properties. "n" is the property name, and "v" is the property value. Example: -property:WarningLevel=2;OutDir=bin\Debug\
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        [ArgKey("")]
        public DotnetMsBuildTask Property(string property)
        {
            WithArgumentsKeyFromAttribute(property, separator: ":");
            return this;
        }

        /// <summary>
        /// Specifies the maximum number of concurrent processes to build with. If the switch is not used, the default value used is 1.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        [ArgKey("-maxCpuCount")]
        public DotnetMsBuildTask MaxCpuCount(int count)
        {
            WithArgumentsKeyFromAttribute(count.ToString(), separator: ":");
            return this;
        }

        /// <summary>
        /// The version of the MSBuild Toolset (tasks, targets, etc.) to use during build. This version will override the versions specified by individual projects.
        /// </summary>
        /// <param name="toolsVersion"></param>
        /// <returns></returns>
        [ArgKey("-toolsVersion")]
        public DotnetMsBuildTask ToolsVersion(string toolsVersion)
        {
            WithArgumentsKeyFromAttribute(toolsVersion, separator: ":");
            return this;
        }

        /// <summary>
        /// Display this amount of information in the event log. The available verbosity levels are: q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic].
        /// </summary>
        /// <param name="verbosity"></param>
        /// <returns></returns>
        [ArgKey("-verbosity")]
        public DotnetMsBuildTask Verbosity(string verbosity)
        {
            WithArgumentsKeyFromAttribute(verbosity, separator: ":");
            return this;
        }

        /// <summary>
        /// Parameters to console logger.
        /// </summary>
        /// <param name="consoleLoggerParameters"></param>
        /// <returns></returns>
        [ArgKey("-consoleLoggerParameters")]
        public DotnetMsBuildTask ConsoleLoggerParameters(string consoleLoggerParameters)
        {
            WithArgumentsKeyFromAttribute(consoleLoggerParameters, separator: ":");
            return this;
        }

        /// <summary>
        /// Disable the default console logger and do not log events to the console.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        [ArgKey("-noConsoleLogger")]
        public DotnetMsBuildTask NoConsoleLogger()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  Logs the build output to a file. By default the file is in the current directory and named "msbuild[n].log".
        /// </summary>
        /// <param name="fileLogger"></param>
        /// <returns></returns>
        [ArgKey("-fileLogger")]
        public DotnetMsBuildTask FileLogger(string fileLogger)
        {
            WithArgumentsKeyFromAttribute(fileLogger, separator: ":");
            return this;
        }

        /// <summary>
        /// Provides any extra parameters for file loggers.
        /// Example: -fileLoggerParameters:LogFile=MyLog.log;Append;Verbosity=diagnostic;Encoding=UTF-8
        /// </summary>
        /// <param name="fileLoggerParameters"></param>
        /// <returns></returns>
        [ArgKey("-fileLoggerParameters")]
        public DotnetMsBuildTask FileLoggerParameters(string fileLoggerParameters)
        {
            WithArgumentsKeyFromAttribute(fileLoggerParameters, separator: ":");
            return this;
        }

        /// <summary>
        /// Use this logger to log events from MSBuild, attaching a different logger instance to each node. To specify multiple loggers, specify each logger separately.
        /// </summary>
        /// <param name="distributedLogger"></param>
        /// <returns></returns>
        [ArgKey("-distributedLogger")]
        public DotnetMsBuildTask DistributedLogger(string distributedLogger)
        {
            WithArgumentsKeyFromAttribute(distributedLogger, separator: ":");
            return this;
        }

        /// <summary>
        /// -distributedLogger:"central logger"*"forwarding logger" Logs the build output to multiple log files, one log file per MSBuild node. The initial location for these files is the current directory.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        [ArgKey("-distributedFileLogger")]
        public DotnetMsBuildTask DistributedFileLogger(string distributedFileLogger)
        {
            WithArgumentsKeyFromAttribute(distributedFileLogger, separator: ":");
            return this;
        }

        /// <summary>
        ///  Use this logger to log events from MSBuild. To specify multiple loggers, specify each logger separately.
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        [ArgKey("-logger")]
        public DotnetMsBuildTask Logger(string logger)
        {
            WithArgumentsKeyFromAttribute(logger, separator: ":");
            return this;
        }

        /// <summary>
        /// -binaryLogger[:[LogFile=]output.binlog[;ProjectImports={None,Embed,ZipFile}]] Serializes all build events to a compressed binary file.
        /// </summary>
        /// <param name="binaryLogger"></param>
        /// <returns></returns>
        [ArgKey("-binaryLogger")]
        public DotnetMsBuildTask BinaryLogger(string binaryLogger)
        {
            WithArgumentsKeyFromAttribute(binaryLogger, separator: ":");
            return this;
        }

        /// <summary>
        /// List of warning codes to treats as low importance messages. Use a semicolon or a comma to separate multiple extensions.
        /// Example: -warnAsMessage:MSB3026
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [ArgKey("-warnAsMessage")]
        public DotnetMsBuildTask WarnAsMessage(string code)
        {
            WithArgumentsKeyFromAttribute(code, separator: ":");
            return this;
        }

        /// <summary>
        /// List of extensions to ignore when determining which project file to build. Use a semicolon or a comma to separate multiple extensions.
        /// Example: -ignoreProjectExtensions:.sln
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        [ArgKey("")]
        public DotnetMsBuildTask IgnoreProjectExtensions(string extension)
        {
            WithArgumentsKeyFromAttribute(extension, separator: ":");
            return this;
        }

        /// <summary>
        /// Enables or Disables the reuse of MSBuild nodes.
        /// </summary>
        /// <param name="nodeReuse"></param>
        /// <returns></returns>
        [ArgKey("-nodeReuse", "-nr")]
        public DotnetMsBuildTask NodeReuse(bool nodeReuse)
        {
            WithArgumentsKeyFromAttribute(nodeReuse.ToString(), separator: ":");
            return this;
        }

        /// <summary>
        /// Creates a single, aggregated project file by inlining all the files that would be imported during a build, with their boundaries marked.
        /// This can be useful for figuring out what files are being imported and from where, and what they will contribute to the build.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [ArgKey("-preprocess")]
        public DotnetMsBuildTask Preprocess(string file)
        {
            WithArgumentsKeyFromAttribute(file, separator: ":");
            return this;
        }

        /// <summary>
        /// Shows detailed information at the end of the build about the configurations built and how they were scheduled to nodes.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        [ArgKey("-detailedSummary")]
        public DotnetMsBuildTask DetailedSummary()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Runs a target named Restore prior to building other targets and ensures the build for these targets uses the latest restored build logic.
        /// This is useful when your project tree requires packages to be restored before it can be built.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        [ArgKey("-restore")]
        public DotnetMsBuildTask Restore()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  Set or override these project-level properties only during restore and do not use properties specified with the -property argument.
        ///  Example: -restoreProperty:IsRestore=true;MyProperty=value
        /// </summary>
        /// <param name="restoreProperty"></param>
        /// <returns></returns>
        [ArgKey("-restoreProperty")]
        public DotnetMsBuildTask RestoreProperty(string restoreProperty)
        {
            WithArgumentsKeyFromAttribute(restoreProperty, separator: ":");
            return this;
        }

        /// <summary>
        ///  Profiles MSBuild evaluation and writes the result to the specified file.
        /// If the extension of the specified file is '.md', the result is generated in markdown format. Otherwise, a tab separated file is produced.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [ArgKey("-profileEvaluation")]
        public DotnetMsBuildTask ProfileEvaluation(string file)
        {
            WithArgumentsKeyFromAttribute(file, separator: ":");
            return this;
        }

        /// <summary>
        /// Indicates that actions in the build are allowed to interact with the user.  Do not use this argument in an automated scenario where interactivity is not expected.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        [ArgKey("-interactive")]
        public DotnetMsBuildTask Interactive()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Causes MSBuild to build each project in isolation.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        [ArgKey("-isolateProjects")]
        public DotnetMsBuildTask IsolateProjects()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  Semicolon separated list of input cache files that MSBuild will read build results from.
        /// </summary>
        /// <param name="cacheFile"></param>
        /// <returns></returns>
        [ArgKey("-inputResultsCaches", "-irc")]
        public DotnetMsBuildTask InputResultsCaches(string cacheFile)
        {
            WithArgumentsKeyFromAttribute(cacheFile, separator: ":");
            return this;
        }

        /// <summary>
        /// Output cache file where MSBuild will write the contents of its build result caches at the end of the build. Setting this also turns on isolated builds (-isolate).
        /// </summary>
        /// <param name="cacheFile"></param>
        /// <returns></returns>
        [ArgKey("-outputResultsCache")]
        public DotnetMsBuildTask OutputResultsCache(string cacheFile)
        {
            WithArgumentsKeyFromAttribute(cacheFile, separator: ":");
            return this;
        }

        /// <summary>
        /// Causes MSBuild to construct and build a project graph.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        [ArgKey("-graphBuild")]
        public DotnetMsBuildTask GraphBuild()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Do not auto-include any MSBuild.rsp files.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        [ArgKey("-noAutoResponse")]
        public DotnetMsBuildTask NoAutoResponse()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }
    }
}
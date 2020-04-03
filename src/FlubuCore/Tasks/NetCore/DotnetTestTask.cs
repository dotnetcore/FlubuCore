using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Process;
using Renci.SshNet.Messages;

namespace FlubuCore.Tasks.NetCore
{
    public class DotnetTestTask : ExecuteDotnetTaskBase<DotnetTestTask>
    {
        private string _description;

        private string _projectName;

        public DotnetTestTask()
            : base(StandardDotnetCommands.Test)
        {
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes dotnet command Test";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// The MSBuild project file to publish. If a project file is not specified, MSBuild searches the current working directory for a file that has a file extension that ends in `proj` and uses that file.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public DotnetTestTask Project(string projectName)
        {
            _projectName = projectName;
            return this;
        }

        /// <summary>
        /// Looks for test binaries for a specific framework.
        /// </summary>
        /// <param name="framework"></param>
        /// <returns></returns>
        [ArgKey("--framework", "-f")]
        public DotnetTestTask Framework(string framework)
        {
            WithArgumentsKeyFromAttribute(framework);
            return this;
        }

        /// <summary>
        /// Looks for test binaries for a specific framework.
        /// </summary>
        /// <param name="framework"></param>
        /// <returns></returns>
        [ArgKey("--framework", "-f")]
        public DotnetTestTask Framework(Framework framework)
        {
            WithArgumentsKeyFromAttribute(framework.ToString());
            return this;
        }

        /// <summary>
        /// Directory in which to find the binaries to be run.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        [ArgKey("--output", "-o")]
        public DotnetTestTask OutputDirectory(string directory)
        {
            WithArgumentsKeyFromAttribute(directory);
            return this;
        }

        /// <summary>
        /// Settings to use when running tests.
        /// </summary>
        /// <param name="settingFilePath"></param>
        /// <returns></returns>
        [ArgKey("--settings")]
        public DotnetTestTask SetSettingFileToUse(string settingFilePath)
        {
            WithArgumentsKeyFromAttribute(settingFilePath);
            return this;
        }

        /// <summary>
        /// Use custom adapters from the given path in the test run.
        /// </summary>
        /// <param name="pathToAdapter"></param>
        /// <returns></returns>
        [ArgKey("--test-adapter-path")]
        public DotnetTestTask SetTestAdapterPath(string pathToAdapter)
        {
            WithArgumentsKeyFromAttribute(pathToAdapter);
            return this;
        }

        /// <summary>
        /// Run tests that match the given expression.
        /// Examples:
        /// Run tests with priority set to 1: --filter "Priority = 1"
        /// Run a test with the specified full name: --filter "FullyQualifiedName=Namespace.ClassName.MethodName"
        /// Run tests that contain the specified name: --filter "FullyQualifiedName~Namespace.Class"
        /// More info on filtering support: https://aka.ms/vstest-filtering
        /// </summary>
        /// <param name="filterExpression"></param>
        /// <returns></returns>
        [ArgKey("--filter")]
        public DotnetTestTask AddFilter(string filterExpression)
        {
            WithArgumentsKeyFromAttribute(filterExpression);
            return this;
        }

        /// <summary>
        /// Configuration to use for building the project. Default for most projects is  "Debug".
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        [ArgKey("--configuration", "-c")]
        public DotnetTestTask Configuration(string configuration)
        {
            WithArgumentsKeyFromAttribute(configuration);
            return this;
        }

        /// <summary>
        /// Configuration to use for building the project. Default for most projects is  "Debug".
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        [ArgKey("--configuration", "-c")]
        public DotnetTestTask Configuration(Configuration configuration)
        {
            WithArgumentsKeyFromAttribute(configuration.ToString());
            return this;
        }

        /// <summary>
        /// Enable verbose logs for test platform. Logs are written to the provided file.
        /// </summary>
        /// <param name="pathToFile"></param>
        /// <returns></returns>
        [ArgKey("--diag", "-d")]
        public DotnetTestTask VerboseLogs(string pathToFile)
        {
            WithArgumentsKeyFromAttribute(pathToFile);
            return this;
        }

        /// <summary>
        /// Do not build project before testing.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-build")]
        public DotnetTestTask NoBuild()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Set the verbosity level of the command.
        /// </summary>
        /// <param name="verbosity"></param>
        /// <returns></returns>
        [ArgKey("--verbosity", "-v")]
        public DotnetTestTask Verbosity(VerbosityOptions verbosity)
        {
            WithArgumentsKeyFromAttribute(verbosity.ToString().ToLower());
            return this;
        }

        /// <summary>
        ///  The logger to use for test results. See https://aka.ms/vstest-report for more information on logger arguments.
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        [ArgKey("--logger")]
        public DotnetTestTask Logger(string logger)
        {
            WithArgumentsKeyFromAttribute(logger);
            return this;
        }

        /// <summary>
        /// The directory where the test results are going to be placed. The specified directory will be created if it does not exist.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [ArgKey("--results-directory", "r")]
        public DotnetTestTask ResultDirectory(string path)
        {
            WithArgumentsKeyFromAttribute(path);
            return this;
        }

        [ArgKey("--collect")]
        public DotnetTestTask Collect(string dataCollectorName)
        {
            WithArgumentsKeyFromAttribute(dataCollectorName);
            return this;
        }

        /// <summary>
        /// Run the tests in blame mode. This option is helpful in isolating a problematic test causing the test host to crash.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--blame")]
        public DotnetTestTask Blame()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  Do not restore the project before building.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-restore")]
        public DotnetTestTask NoRestore()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// List the discovered tests instead of running the tests.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--list-tests")]
        public DotnetTestTask ListTests()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        protected override void BeforeExecute(ITaskContextInternal context, IRunProgramTask runProgramTask)
        {
            var args = GetArguments();
            if (string.IsNullOrEmpty(_projectName))
            {
                if (args.Count == 0 || args[0].StartsWith("-"))
                {
                    var solustionFileName = context.Properties.Get<string>(BuildProps.SolutionFileName, null);
                    if (solustionFileName != null)
                    {
                        InsertArgument(0, solustionFileName);
                    }
                }
            }
            else
            {
                InsertArgument(0, _projectName);
            }

            if (!args.Exists(x => x == "-c" || x == "--configuration"))
            {
                var configuration = context.Properties.Get<string>(BuildProps.BuildConfiguration, null);
                if (configuration != null)
                {
                    Configuration(configuration);
                }
            }

            base.BeforeExecute(context, runProgramTask);
        }
    }
}
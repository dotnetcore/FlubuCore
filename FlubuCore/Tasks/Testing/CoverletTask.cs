using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Testing
{
    public class CoverletTask : ExternalProcessTaskBase<int, CoverletTask>
    {
        private string _description;

        public CoverletTask(string assembly)
        {
            ExecutablePath = "coverlet";
            InsertArgument(0, assembly);
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes coverlet. Coverlet is a cross platform code coverage library for .NET Core, with support for line, branch and method coverage.";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// Path to the test runner application.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public CoverletTask Target(string target)
        {
            WithArguments("--target", target);
            return this;
        }

        /// <summary>
        /// Arguments to be passed to the test runner.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public CoverletTask TargetArgs(params string[] args)
        {
            WithArguments("--target args", $"\"{string.Join(" ", args)}\"");
            return this;
        }

        /// <summary>
        /// Output of the generated coverage report.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public CoverletTask Output(string output)
        {
            WithArguments("--output", output);
            return this;
        }

        /// <summary>
        ///  Sets the verbosity level of the command. Allowed values are quiet, minimal, normal, detailed.
        /// </summary>
        /// <param name="verbosity"></param>
        /// <returns></returns>
        public CoverletTask Verbosity(string verbosity)
        {
            WithArguments("--verbosity", verbosity);
            return this;
        }

        /// <summary>
        /// Format of the generated coverage report.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public CoverletTask Format(string format)
        {
            WithArguments("--format", format);
            return this;
        }

        /// <summary>
        /// Exits with error if the coverage % is below value.
        /// </summary>
        /// <param name="treshold"></param>
        /// <returns></returns>
        public CoverletTask Treshold(int treshold)
        {
            WithArguments("--treshold", treshold.ToString());
            return this;
        }

        /// <summary>
        /// Coverage type to apply the threshold to.
        /// </summary>
        /// <param name="tresholdType"></param>
        /// <returns></returns>
        public CoverletTask TresholdType(string tresholdType)
        {
            WithArguments("--treshold-type", tresholdType);
            return this;
        }

        /// <summary>
        /// Coverage statistic used to enforce the threshold value.
        /// </summary>
        /// <param name="tresholdStat"></param>
        /// <returns></returns>
        public CoverletTask TresholdStat(string tresholdStat)
        {
            WithArguments("--treshold-stat", tresholdStat);
            return this;
        }

        /// <summary>
        ///  Filter expressions to exclude specific modules and types.
        /// </summary>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public CoverletTask Exclude(string exclude)
        {
            WithArguments("--exclude", exclude);
            return this;
        }

        /// <summary>
        /// Filter expressions to include specific modules and types.
        /// </summary>
        /// <param name="include"></param>
        /// <returns></returns>
        public CoverletTask Include(string include)
        {
            WithArguments("--include");
            return this;
        }

        /// <summary>
        /// Include directories containing additional assemblies to be instrumented.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public CoverletTask IncludeDirectory(string directory)
        {
            WithArguments("--include-directory", directory);
            return this;
        }

        /// <summary>
        /// Glob patterns specifying source files to exclude.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public CoverletTask ExludeByFile(string file)
        {
            WithArguments("--exclude-by-file", file);
            return this;
        }

        /// <summary>
        /// Attributes to exclude from code coverage.
        /// </summary>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public CoverletTask ExcludeByAttribute(string exclude)
        {
            WithArguments("Attributes to exclude from code coverage");
            return this;
        }

        /// <summary>
        /// Specifies whether to report code coverage of the test assembly.
        /// </summary>
        /// <returns></returns>
        public CoverletTask IncludeTestAssembly()
        {
            WithArguments("--include-test-assembly");
            return this;
        }

        /// <summary>
        /// Specifies whether to limit code coverage hit reporting to a single hit for each location.
        /// </summary>
        /// <returns></returns>
        public CoverletTask SingeHit()
        {
            WithArguments("--single-hit");
            return this;
        }

        /// <summary>
        /// Path to existing coverage result to merge.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public CoverletTask MergeWith(string path)
        {
            WithArguments("--merge-with", path);
            return this;
        }
    }
}

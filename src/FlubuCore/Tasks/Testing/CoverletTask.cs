using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks.Attributes;
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
            AddAdditionalOptionPrefix("Coverlet");
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
        [ArgKey("--target")]
        public CoverletTask Target(string target)
        {
            WithArgumentsKeyFromAttribute(target);
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
        [ArgKey("--output")]
        public CoverletTask Output(string output)
        {
            WithArgumentsKeyFromAttribute(output);
            return this;
        }

        /// <summary>
        ///  Sets the verbosity level of the command. Allowed values are quiet, minimal, normal, detailed.
        /// </summary>
        /// <param name="verbosity"></param>
        /// <returns></returns>
        [ArgKey("--verbosity")]
        public CoverletTask Verbosity(string verbosity)
        {
            WithArgumentsKeyFromAttribute(verbosity);
            return this;
        }

        /// <summary>
        /// Format of the generated coverage report.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        [ArgKey("--format")]
        public CoverletTask Format(string format)
        {
            WithArgumentsKeyFromAttribute(format);
            return this;
        }

        /// <summary>
        /// Exits with error if the coverage % is below value.
        /// </summary>
        /// <param name="treshold"></param>
        /// <returns></returns>
        [ArgKey("--treshold")]
        public CoverletTask Treshold(int treshold)
        {
            WithArgumentsKeyFromAttribute(treshold.ToString());
            return this;
        }

        /// <summary>
        /// Coverage type to apply the threshold to.
        /// </summary>
        /// <param name="tresholdType"></param>
        /// <returns></returns>
        [ArgKey("--treshold-type")]
        public CoverletTask TresholdType(string tresholdType)
        {
            WithArgumentsKeyFromAttribute(tresholdType);
            return this;
        }

        /// <summary>
        /// Coverage statistic used to enforce the threshold value.
        /// </summary>
        /// <param name="tresholdStat"></param>
        /// <returns></returns>
        [ArgKey("--treshold-stat")]
        public CoverletTask TresholdStat(string tresholdStat)
        {
            WithArgumentsKeyFromAttribute(tresholdStat);
            return this;
        }

        /// <summary>
        ///  Filter expressions to exclude specific modules and types.
        /// </summary>
        /// <param name="exclude"></param>
        /// <returns></returns>
        [ArgKey("--exclude")]
        public CoverletTask Exclude(string exclude)
        {
            WithArgumentsKeyFromAttribute(exclude);
            return this;
        }

        /// <summary>
        /// Filter expressions to include specific modules and types.
        /// </summary>
        /// <param name="include"></param>
        /// <returns></returns>
        [ArgKey("--include")]
        public CoverletTask Include(string include)
        {
            WithArgumentsKeyFromAttribute(include);
            return this;
        }

        /// <summary>
        /// Include directories containing additional assemblies to be instrumented.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        [ArgKey("--include-directory")]
        public CoverletTask IncludeDirectory(string directory)
        {
            WithArgumentsKeyFromAttribute(directory);
            return this;
        }

        /// <summary>
        /// Glob patterns specifying source files to exclude.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [ArgKey("--exclude-by-file")]
        public CoverletTask ExludeByFile(string file)
        {
            WithArgumentsKeyFromAttribute(file);
            return this;
        }

        /// <summary>
        /// Attributes to exclude from code coverage.
        /// </summary>
        /// <param name="exclude"></param>
        /// <returns></returns>
        [ArgKey("exclude-by-attribute")]
        public CoverletTask ExcludeByAttribute(string exclude)
        {
            WithArgumentsKeyFromAttribute(exclude);
            return this;
        }

        /// <summary>
        /// Specifies whether to report code coverage of the test assembly.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--include-test-assembly")]
        public CoverletTask IncludeTestAssembly()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Specifies whether to limit code coverage hit reporting to a single hit for each location.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--single-hit")]
        public CoverletTask SingeHit()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Path to existing coverage result to merge.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [ArgKey("--merge-with")]
        public CoverletTask MergeWith(string path)
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            AddPrefixToAdditionalOptionKey(PrefixProcessors.AddSlashPrefixToAdditionalOptionKey);
            ChangeAdditionalOptionKeyValueSeperator(' ');
            return base.DoExecute(context);
        }
    }
}

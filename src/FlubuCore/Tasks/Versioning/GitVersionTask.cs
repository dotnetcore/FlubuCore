using System;
using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Process;
using Newtonsoft.Json;

namespace FlubuCore.Tasks.Versioning
{
    public class GitVersionTask : ExternalProcessTaskBase<GitVersion, GitVersionTask>
    {
        public GitVersionTask()
        {
            ExecutablePath = "gitversion";
            KeepProgramOutput = true;
            AddAdditionalOptionPrefix("/GitVersion:");
            AddAdditionalOptionPrefix("/GitVer:");
        }

        protected override string Description { get; set; }

        /// <summary>
        ///  The directory containing .git. If not defined current directory is used. (Must be first argument).
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public GitVersionTask Path(string path)
        {
            InsertArgument(0, path);
            return this;
        }

        /// <summary>
        /// Runs GitVersion with additional diagnostic information (requires git.exe to be installed)
        /// </summary>
        /// <returns></returns>
        [ArgKey("/diag")]
        public GitVersionTask Diagnostic()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Determines the output to the console. Can be either 'json' or 'buildserver', will default to 'json'.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        [ArgKey("/output")]
        public GitVersionTask Output(GitVersionOutput output)
        {
            WithArgumentsKeyFromAttribute(output.ToString().ToLower());
            return this;
        }

        /// <summary>
        /// Used in conjuntion with /output json, will output just a particular variable.
        /// eg /output json /showvariable SemVer - will output `1.2.3+beta.4`
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        [ArgKey("/showvariable")]
        public GitVersionTask ShowVariable(string variable)
        {
            WithArgumentsKeyFromAttribute(variable);
            return this;
        }

        /// <summary>
        /// Path to logfile.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [ArgKey("/l")]
        public GitVersionTask LogFile(string path)
        {
            WithArgumentsKeyFromAttribute(path);
            return this;
        }

        /// <summary>
        /// Outputs the effective GitVersion config (defaults + custom from GitVersion.yml) in yaml format.
        /// </summary>
        /// <returns></returns>
        [ArgKey("/showconfig")]
        public GitVersionTask ShowConfig()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Overrides GitVersion config values inline (semicolon-separated key value pairs e.g. /overrideconfig tag-prefix=Foo)
        /// </summary>
        /// <param name="configs"></param>
        /// <returns></returns>
        public GitVersionTask OverrideConfig(Dictionary<string, string> configs)
        {
            if (configs != null && configs.Count != 0)
            {
                WithArguments("/overrideconfig");
                foreach (var config in configs)
                {
                    WithArguments($"{config.Key}={config.Value}");
                }
            }

            return this;
        }

        /// <summary>
        ///  Bypasses the cache, result will not be written to the cache.
        /// </summary>
        /// <returns></returns>
        [ArgKey("/nocache")]
        public GitVersionTask NoCache()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  Will recursively search for all 'AssemblyInfo.cs' files in the git repo and update them.
        /// </summary>
        /// <returns></returns>
        [ArgKey("/updateassemblyinfo")]
        public GitVersionTask UpdateAssemblyInfo()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Specify name of AssemblyInfo file. Can also /updateAssemblyInfo GlobalAssemblyInfo.cs as a shorthand
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [ArgKey("/updateassemblyinfofilename")]
        public GitVersionTask UpdateAssemblyInfoFilename(string fileName)
        {
            WithArgumentsKeyFromAttribute(fileName);
            return this;
        }

        /// <summary>
        ///    If the assembly info file specified with /updateassemblyinfo or /updateassemblyinfofilename is not found,
        /// it be created with these attributes: AssemblyFileVersion, AssemblyVersion and AssemblyInformationalVersion
        /// </summary>
        /// <returns></returns>
        [ArgKey("/ensureassemblyinfo")]
        public GitVersionTask EnsureAssemblyInfo()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Url to remote git repository.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [ArgKey("/url")]
        public GitVersionTask GitUrl(string url)
        {
            WithArgumentsKeyFromAttribute(url);
            return this;
        }

        /// <summary>
        /// Name of the branch to use on the remote repository, must be used in combination with /url.
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        [ArgKey("/b")]
        public GitVersionTask Branch(string branch)
        {
            WithArgumentsKeyFromAttribute(branch);
            return this;
        }

        /// <summary>
        /// Username in case authentication is required.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [ArgKey("/u")]
        public GitVersionTask Username(string username)
        {
            WithArgumentsKeyFromAttribute(username);
            return this;
        }

        /// <summary>
        /// Password in case authentication is required.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        [ArgKey("/p")]
        public GitVersionTask Password(string password)
        {
            WithArgumentsKeyFromAttribute(password, true);
            return this;
        }

        /// <summary>
        /// The commit id to check. If not specified, the latest available commit on the specified branch will be used.
        /// </summary>
        /// <param name="commitId"></param>
        /// <returns></returns>
        [ArgKey("/c")]
        public GitVersionTask Commit(string commitId)
        {
            WithArgumentsKeyFromAttribute(commitId);
            return this;
        }

        /// <summary>
        ///  By default dynamic repositories will be cloned to %tmp%. Use this switch to override
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        [ArgKey("/dynamicRepoLocation")]
        public GitVersionTask DynamicRepoLocation(string location)
        {
            WithArgumentsKeyFromAttribute(location);
            return this;
        }

        /// <summary>
        ///  Disables 'git fetch' during version calculation. Might cause GitVersion to not calculate your version as expected.
        /// </summary>
        /// <returns></returns>
        [ArgKey("/nofetch")]
        public GitVersionTask NoFetch()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Executes target executable making GitVersion variables available as environmental variables.
        /// </summary>
        /// <param name="executable"></param>
        /// <returns></returns>
        [ArgKey("/exec")]
        public GitVersionTask Executable(string executable)
        {
            WithArgumentsKeyFromAttribute(executable);
            return this;
        }

        /// <summary>
        /// Arguments for the executable specified by /exec.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        [ArgKey("/execargs")]
        public GitVersionTask ExecutableArguments(string arguments)
        {
            WithArgumentsKeyFromAttribute(arguments);
            return this;
        }

        /// <summary>
        ///  Build a msbuild file, GitVersion variables will be passed as msbuild properties
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [ArgKey("/proj")]
        public GitVersionTask MsBuildProject(string file)
        {
            WithArgumentsKeyFromAttribute(file);
            return this;
        }

        /// <summary>
        /// Additional arguments to pass to msbuild.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        [ArgKey("/projargs")]
        public GitVersionTask MsBuildProjectArguments(string arguments)
        {
            WithArgumentsKeyFromAttribute(arguments);
            return this;
        }

        /// <summary>
        /// Set Verbosity level (debug, info, warn, error, none). Default is info
        /// </summary>
        /// <param name="verbosity"></param>
        /// <returns></returns>
        [ArgKey("/verbosity")]
        public GitVersionTask Verbosity(string verbosity)
        {
            WithArgumentsKeyFromAttribute(verbosity);
            return this;
        }

        protected override GitVersion DoExecute(ITaskContextInternal context)
        {
            AddPrefixToAdditionalOptionKey(PrefixProcessors.AddSlashPrefixToAdditionalOptionKey);
            ChangeAdditionalOptionKeyValueSeperator(' ');
            base.DoExecute(context);

            var output = GetOutput();
            if (!string.IsNullOrEmpty(output))
            {
                var gitVersion = JsonConvert.DeserializeObject<GitVersion>(output);

                var buildVersion = new BuildVersion
                {
                    Version = new Version(gitVersion.AssemblySemVer),
                };

                context.Properties.Set(BuildProps.BuildVersion, buildVersion);
                return gitVersion;
            }

            return null;
        }
    }
}
using System;
using System.Collections.Generic;
using FlubuCore.Context;
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
        public GitVersionTask Diagnostic()
        {
            WithArguments("/diag");
            return this;
        }

        /// <summary>
        /// Determines the output to the console. Can be either 'json' or 'buildserver', will default to 'json'.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public GitVersionTask Output(GitVersionOutput output)
        {
            WithArguments("/output", output.ToString().ToLower());
            return this;
        }

        /// <summary>
        /// Used in conjuntion with /output json, will output just a particular variable.
        /// eg /output json /showvariable SemVer - will output `1.2.3+beta.4`
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public GitVersionTask ShowVariable(string variable)
        {
            WithArguments("/showvariable");
            return this;
        }

        /// <summary>
        /// Path to logfile.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public GitVersionTask LogFile(string path)
        {
            WithArguments("/l", path);
            return this;
        }

        /// <summary>
        /// Outputs the effective GitVersion config (defaults + custom from GitVersion.yml) in yaml format.
        /// </summary>
        /// <returns></returns>
        public GitVersionTask ShowConfig()
        {
            WithArguments("/showconfig");
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
        public GitVersionTask NoCache()
        {
            WithArguments("/nocache");
            return this;
        }

        /// <summary>
        ///  Will recursively search for all 'AssemblyInfo.cs' files in the git repo and update them.
        /// </summary>
        /// <returns></returns>
        public GitVersionTask UpdateAssemblyInfo()
        {
            WithArguments("/updateassemblyinfo");
            return this;
        }

        /// <summary>
        /// Specify name of AssemblyInfo file. Can also /updateAssemblyInfo GlobalAssemblyInfo.cs as a shorthand
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public GitVersionTask UpdateAssemblyInfoFilename(string fileName)
        {
            WithArguments(" /updateassemblyinfofilename", fileName);
            return this;
        }

        /// <summary>
        ///    If the assembly info file specified with /updateassemblyinfo or /updateassemblyinfofilename is not found,
        /// it be created with these attributes: AssemblyFileVersion, AssemblyVersion and AssemblyInformationalVersion
        /// </summary>
        /// <returns></returns>
        public GitVersionTask EnsureAssemblyInfo()
        {
            WithArguments("/ensureassemblyinfo");
            return this;
        }

        /// <summary>
        /// Url to remote git repository.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public GitVersionTask GitUrl(string url)
        {
            WithArguments("/url", url);
            return this;
        }

        /// <summary>
        /// Name of the branch to use on the remote repository, must be used in combination with /url.
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        public GitVersionTask Branch(string branch)
        {
            WithArguments("/b", branch);
            return this;
        }

        /// <summary>
        /// Username in case authentication is required.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public GitVersionTask Username(string username)
        {
            WithArguments("/u", username);
            return this;
        }

        /// <summary>
        /// Password in case authentication is required.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public GitVersionTask Password(string password)
        {
            WithArguments("/p");
            WithArguments(password, true);
            return this;
        }

        /// <summary>
        /// The commit id to check. If not specified, the latest available commit on the specified branch will be used.
        /// </summary>
        /// <param name="commitId"></param>
        /// <returns></returns>
        public GitVersionTask Commit(string commitId)
        {
            WithArguments("/c", commitId);
            return this;
        }

        /// <summary>
        ///  By default dynamic repositories will be cloned to %tmp%. Use this switch to override
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public GitVersionTask DynamicRepoLocation(string location)
        {
            WithArguments("/dynamicRepoLocation", location);
            return this;
        }

        /// <summary>
        ///  Disables 'git fetch' during version calculation. Might cause GitVersion to not calculate your version as expected.
        /// </summary>
        /// <returns></returns>
        public GitVersionTask NoFetch()
        {
            WithArguments("/nofetch");
            return this;
        }

        /// <summary>
        /// Executes target executable making GitVersion variables available as environmental variables.
        /// </summary>
        /// <param name="executable"></param>
        /// <returns></returns>
        public GitVersionTask Executable(string executable)
        {
            WithArguments("/exec", executable);
            return this;
        }

        /// <summary>
        /// Arguments for the executable specified by /exec.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public GitVersionTask ExecutableArguments(string arguments)
        {
            WithArguments("/execargs", arguments);
            return this;
        }

        /// <summary>
        ///  Build a msbuild file, GitVersion variables will be passed as msbuild properties
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public GitVersionTask MsBuildProject(string file)
        {
            WithArguments("/proj", file);
            return this;
        }

        /// <summary>
        /// Additional arguments to pass to msbuild.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public GitVersionTask MsBuildProjectArguments(string arguments)
        {
            WithArguments("/projargs", arguments);
            return this;
        }

        /// <summary>
        /// Set Verbosity level (debug, info, warn, error, none). Default is info
        /// </summary>
        /// <param name="verbosity"></param>
        /// <returns></returns>
        public GitVersionTask Verbosity(string verbosity)
        {
            WithArguments("/verbosity", verbosity);
            return this;
        }

        protected override GitVersion DoExecute(ITaskContextInternal context)
        {
            base.DoExecute(context);

            var output = GetOutput();
            if (!string.IsNullOrEmpty(output))
            {
                var gitVersion = JsonConvert.DeserializeObject<GitVersion>(output);
                context.Properties.Set(BuildProps.BuildVersion, new Version(gitVersion.AssemblySemVer));
                return gitVersion;
            }

            return null;
        }
    }
}

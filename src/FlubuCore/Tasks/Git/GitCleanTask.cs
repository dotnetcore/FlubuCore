using FlubuCore.Context;
using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Git
{
    public class GitCleanTask : ExternalProcessTaskBase<int, GitCleanTask>
    {
        private string _description;
        private string[] _paths;

        /// <summary>
        /// Task removes untracked files from the working tree
        /// </summary>
        public GitCleanTask()
        {
             ExecutablePath = "git";
             InsertArgument(0, "clean");
             AddPrefixToAdditionalOptionKey(PrefixProcessors.AddDoubleDashPrefixToAdditionalOptionKey);
             AddAdditionalOptionPrefix("Clean");
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes git command 'clean' with specified option.";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// Cleans only specified paths instead of the full repository.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public GitCleanTask SetAffectedPaths(params string[] paths)
        {
            _paths = paths;
            return this;
        }

        /// <summary>
        /// If the Git configuration variable clean.requireForce is not set to false,
        /// git clean will refuse to delete files or directories unless using Force mode.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--force", "-f")]
        public GitCleanTask Force()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Normally, when no <path> is specified, git clean will not recurse into untracked directories to avoid removing too much.
        /// Use WithUntrackedDirectories to have it recurse into such directories as well.
        /// If any paths are specified, this option is irrelevant; all untracked files matching the specified paths will be removed.
        /// </summary>
        /// <returns></returns>
        [ArgKey("-d")]
        public GitCleanTask WithUntrackedDirectories()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Don’t use the standard ignore rules (see gitignore docs),
        /// but still use the ignore rules given with Exclude option.
        /// This allows removing all untracked files, including build products.
        /// This can be used (possibly in conjunction with git restore or git reset)
        /// to create a pristine working directory to test a clean build.
        /// </summary>
        /// <returns></returns>
        [ArgKey("-x")]
        public GitCleanTask AvoidStandardIgnoreRules()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Remove only files ignored by Git. This may be useful to rebuild everything from scratch, but keep manually created files.
        /// </summary>
        /// <returns></returns>
        [ArgKey("-X")]
        public GitCleanTask CleanIgnoredFilesOnly()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Use the given exclude pattern in addition to the standard ignore rules (see gitignore docs).
        /// </summary>
        /// <returns></returns>
        public GitCleanTask ExcludePattern(string pattern)
        {
            WithArguments("-e", pattern);
            return this;
        }

        /// <summary>
        /// Be quiet, only report errors, but not the files that are successfully removed.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--quiet", "-q")]
        public GitCleanTask Quiet()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Add modified contents in the working tree interactively to the index. Optional path arguments may be supplied to limit operation to a subset of the working tree.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--interactive", "-i")]
        public GitCleanTask Interactive()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            if (_paths != null && _paths.Length > 0)
            {
                WithArguments("--");
                WithArguments(_paths);
            }

            return base.DoExecute(context);
        }
    }
}

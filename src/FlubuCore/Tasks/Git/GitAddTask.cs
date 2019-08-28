using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Git
{
    public class GitAddTask : ExternalProcessTaskBase<int, GitAddTask>
    {
        private string _description;

        /// <summary>
        /// Task updates the index using the current content found in the working tree, to prepare the content staged for the next commit.
        /// </summary>
        public GitAddTask()
        {
             ExecutablePath = "git";
             InsertArgument(0, "add");
             AddPrefixToAdditionalOptionKey(PrefixProcessors.AddDoubleDashPrefixToAdditionalOptionKey);
             AddAdditionalOptionPrefix("Add");
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes git command 'add' with specified option.";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// Files to add content from. Fileglobs (e.g. *.c) can be given to add all matching files.
        /// Also a leading directory name (e.g. dir to add dir/file1 and dir/file2) can be given
        /// to update the index to match the current state of the directory as a whole.
        /// </summary>
        /// <param name="pathSpec"></param>
        /// <returns></returns>
        public GitAddTask PathSpec(string pathSpec)
        {
            InsertArgument(1, pathSpec);
            return this;
        }

        /// <summary>
        /// Be verbose.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--verbose", "-v")]
        public GitAddTask Verbose()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Don’t actually add the file(s), just show if they exist and/or will be ignored.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--dry-run", "-n")]
        public GitAddTask DryRun()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// allow adding otherwise ignored files.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--force", "-f")]
        public GitAddTask Force()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// just skip files which cannot be added because of errors.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--ignore-errors")]
        public GitAddTask IgnoreErrors()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Update the index just where it already has an entry matching <pathspec>. This removes as well as modifies index entries to match the working tree, but adds no new files.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--update", "-u")]
        public GitAddTask Update()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Add modified contents in the working tree interactively to the index. Optional path arguments may be supplied to limit operation to a subset of the working tree.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--interactive", "-i")]
        public GitAddTask Interactive()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Interactively choose hunks of patch between the index and the work tree and add them to the index. This gives the user a chance to review the difference before adding modified contents to the index.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--patch", "-P")]
        public GitAddTask Patch()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Update the index by adding new files that are unknown to the index and files modified in the working tree, but ignore files that have been removed from the working tree.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--ignore-removal", "-no-all")]
        public GitAddTask IgnoreRemoval()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }
    }
}

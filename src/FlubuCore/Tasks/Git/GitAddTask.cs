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
        [ArgKey("--verbose")]
        public GitAddTask Verbose()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Don’t actually add the file(s), just show if they exist and/or will be ignored.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--dry-run")]
        public GitAddTask DryRun()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// allow adding otherwise ignored files
        /// </summary>
        /// <returns></returns>
        [ArgKey("--force")]
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
    }
}

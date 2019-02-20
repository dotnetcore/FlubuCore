using System;
using System.Collections.Generic;
using System.Text;
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
        public GitAddTask Verbose()
        {
            WithArguments("--verbose");
            return this;
        }

        /// <summary>
        /// Don’t actually add the file(s), just show if they exist and/or will be ignored.
        /// </summary>
        /// <returns></returns>
        public GitAddTask DryRun()
        {
            WithArguments("--dry-run");
            return this;
        }

        /// <summary>
        /// allow adding otherwise ignored files
        /// </summary>
        /// <returns></returns>
        public GitAddTask Force()
        {
            WithArguments("--force");
            return this;
        }

        /// <summary>
        /// just skip files which cannot be added because of errors.
        /// </summary>
        /// <returns></returns>
        public GitAddTask IgnoreErrors()
        {
            WithArguments("--ignore-errors");
            return this;
        }
    }
}

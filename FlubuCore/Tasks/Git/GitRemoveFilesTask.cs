using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Git
{
    public class GitRemoveFilesTask : ExternalProcessTaskBase<int, GitTagTask>
    {
        private readonly string _file;

        private string _description;

        /// <summary>
        /// Remove files from the index, or from the working tree and the index. git rm will not remove a file from just your working directory.
        /// </summary>
        public GitRemoveFilesTask(string file)
        {
             _file = file;
             ExecutablePath = "git";
             InsertArgument(0, "rm");
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes git command 'rm' with specified option.";
                }

                return _description;
            }

            set { _description = value; }
        }

        public GitRemoveFilesTask AllowRecursiveRemoval()
        {
            WithArguments("-r");
            return this;
        }

        public GitRemoveFilesTask IgnoreUnmatch()
        {
            WithArguments("--ignore-unmatch");
            return this;
        }

        public GitRemoveFilesTask OnlyRemoveFromTheIndex()
        {
            WithArguments("--cached");
            return this;
        }

        /// <summary>
        /// override the up-to-date check
        /// </summary>
        /// <returns></returns>
        public GitRemoveFilesTask Force()
        {
            WithArguments("--force");
            return this;
        }

           /// <summary>
        /// be more quiet.
        /// </summary>
        /// <returns></returns>
        public GitRemoveFilesTask Quiet()
        {
            WithArguments("--quit");
            return this;
        }

        /// <summary>
        /// Do everything except actually send the updates.
        /// </summary>
        /// <returns></returns>
        public GitRemoveFilesTask DryRun()
        {
            WithArguments("--dry-run");
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _file.MustNotBeNullOrEmpty("Files to remove must not be empty.");
            WithArguments(_file);
            return base.DoExecute(context);
        }
    }
}

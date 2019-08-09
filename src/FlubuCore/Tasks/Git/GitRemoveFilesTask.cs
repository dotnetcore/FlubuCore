using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Git
{
    public class GitRemoveFilesTask : ExternalProcessTaskBase<int, GitRemoveFilesTask>
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
             AddPrefixToAdditionalOptionKey(PrefixProcessors.AddDoubleDashPrefixToAdditionalOptionKey);
             AddAdditionalOptionPrefix("GitRemove");
             AddAdditionalOptionPrefix("GitRm");
             AddAdditionalOptionPrefix("Rm");
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

        [ArgKey("-r")]
        public GitRemoveFilesTask AllowRecursiveRemoval()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        [ArgKey("--ignore-unmatch")]
        public GitRemoveFilesTask IgnoreUnmatch()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        [ArgKey("--cached")]
        public GitRemoveFilesTask OnlyRemoveFromTheIndex()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// override the up-to-date check
        /// </summary>
        /// <returns></returns>
        [ArgKey("--force")]
        public GitRemoveFilesTask Force()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

           /// <summary>
        /// be more quiet.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--quiet")]
        public GitRemoveFilesTask Quiet()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Do everything except actually send the updates.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--dry-run")]
        public GitRemoveFilesTask DryRun()
        {
            WithArgumentsKeyFromAttribute();
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

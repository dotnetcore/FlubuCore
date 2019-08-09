using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Git
{
    public class GitPullTask : ExternalProcessTaskBase<int, GitPullTask>
    {
        private string _description;

        public GitPullTask()
        {
            ExecutablePath = "git";
            InsertArgument(0, "pull");
            AddPrefixToAdditionalOptionKey(PrefixProcessors.AddDoubleDashPrefixToAdditionalOptionKey);
            AddAdditionalOptionPrefix("Pull");
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes git command 'pull' with specified option.";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// force progress reporting.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--progress")]
        public GitPullTask ShowProgress()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// be more verbose.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--verbose")]
        public GitPullTask Verbose()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// be more quiet.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--quiet")]
        public GitPullTask Quiet()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// create a single commit instead of doing a merge.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--squash")]
        public GitPullTask Squash()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }
    }
}

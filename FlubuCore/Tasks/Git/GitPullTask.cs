using System;
using System.Collections.Generic;
using System.Text;
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
        public GitPullTask ShowProgress()
        {
            WithArguments("--progress");
            return this;
        }

        /// <summary>
        /// be more verbose.
        /// </summary>
        /// <returns></returns>
        public GitPullTask Verbose()
        {
            WithArguments("--verbose");
            return this;
        }

        /// <summary>
        /// be more quiet.
        /// </summary>
        /// <returns></returns>
        public GitPullTask Quiet()
        {
            WithArguments("--quit");
            return this;
        }

        /// <summary>
        /// create a single commit instead of doing a merge.
        /// </summary>
        /// <returns></returns>
        public GitPullTask Squash()
        {
            WithArguments("--squash");
            return this;
        }
    }
}

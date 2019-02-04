using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Git
{
    public class GitPushTask : ExternalProcessTaskBase<int, GitPullTask>
    {
        private string _description;

         public GitPushTask()
         {
             ExecutablePath = "git";
             InsertArgument(0, "push");
         }

           protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes git command 'push' with specified option.";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// The "remote" repository that is destination of a push operation.
        /// This parameter can be either a URL or the name of a remote.
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        public GitPushTask Repository(string repository)
        {
             InsertArgument(1, repository);
             return this;
        }

        /// <summary>
        /// Specify what destination ref to update with what source object. See https://git-scm.com/docs/git-push for more info
        /// </summary>
        /// <param name="refSpec"></param>
        /// <returns></returns>
        public GitPushTask RefSpec(string refSpec)
        {
            InsertArgument(2, refSpec);
            return this;
        }

        /// <summary>
        /// Be verbose.
        /// </summary>
        /// <returns></returns>
        public GitPushTask Verbose()
        {
            WithArguments("--verbose");
            return this;
        }

           /// <summary>
        /// be more quiet.
        /// </summary>
        /// <returns></returns>
        public GitPushTask Quiet()
        {
            WithArguments("--quit");
            return this;
        }

        /// <summary>
        /// Do everything except actually send the updates.
        /// </summary>
        /// <returns></returns>
        public GitPushTask DryRun()
        {
            WithArguments("--dry-run");
            return this;
        }

        /// <summary>
        /// force updates.
        /// </summary>
        /// <returns></returns>
        public GitPushTask Force()
        {
            WithArguments("--force");
            return this;
        }
    }
}

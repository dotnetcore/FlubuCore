using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Git
{
    public class GitPushTask : ExternalProcessTaskBase<int, GitPushTask>
    {
        private string _description;

        public GitPushTask()
        {
            ExecutablePath = "git";
            InsertArgument(0, "push");
            AddPrefixToAdditionalOptionKey(PrefixProcessors.AddDoubleDashPrefixToAdditionalOptionKey);
            AddAdditionalOptionPrefix("Push");
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
        [ArgKey("--verbose", "-v")]
        public GitPushTask Verbose()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// be more quiet.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--quiet", "-q")]
        public GitPushTask Quiet()
        {
           WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Do everything except actually send the updates.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--dry-run", "-n")]
        public GitPushTask DryRun()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// force updates.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--force", "-f")]
        public GitPushTask Force()
        {
           WithArgumentsKeyFromAttribute();
           return this;
        }

        /// <summary>
        /// All refs under refs/tags are pushed.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--tags")]
        public GitPushTask Tags()
        {
           WithArgumentsKeyFromAttribute();
           return this;
        }

        /// <summary>
        /// Push all the refs that would be pushed without this option, and also push annotated tags in refs/tags that are missing from the remote but are pointing at commit-ish that are reachable from the refs being pushed.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--follow-tags")]
        public GitPushTask FollowTags()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        [ArgKey("--delete", "-d")]
        public GitPushTask Delete()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        [ArgKey("--force-with-lease")]
        public GitPushTask ForceWithLease(string refName = null)
        {
            if (string.IsNullOrEmpty(refName))
            {
                WithArgumentsKeyFromAttribute();
            }
            else
            {
                WithArgumentsKeyFromAttribute(refName);
            }

            return this;
        }

        /// <summary>
        /// For every branch that is up to date or successfully pushed, add upstream (tracking) reference, used by argument-less git-pull[1] and other commands.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--set-upstream", "-u")]
        public GitPushTask SetUpstream()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }
    }
}

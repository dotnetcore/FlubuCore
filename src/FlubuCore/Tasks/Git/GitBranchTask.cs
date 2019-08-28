using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Git
{
    public class GitBranchTask : ExternalProcessTaskBase<int, GitBranchTask>
    {
        private string _description;

        public GitBranchTask()
        {
            ExecutablePath = "git";
            InsertArgument(0, "branch");
            AddPrefixToAdditionalOptionKey(PrefixProcessors.AddDoubleDashPrefixToAdditionalOptionKey);
            AddAdditionalOptionPrefix("branch");
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes git command 'branch' with specified option.";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// When in list mode, show sha1 and commit subject line for each head, along with relationship to upstream branch (if any).
        /// </summary>
        /// <returns></returns>
        [ArgKey("--verbose")]
        public GitBranchTask Verbose()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Be more quiet when creating or deleting a branch, suppressing non-error messages.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--quiet")]
        public GitBranchTask Quiet()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Delete a branch. The branch must be fully merged in its upstream branch, or in HEAD if no upstream was set with --track or --set-upstream-to.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--delete", "-d")]
        public GitBranchTask Delete()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Reset 'branchname' to 'startpoint', even if 'branchname' exists already. Without -f, git branch refuses to change an existing branch.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--force", "-f")]
        public GitBranchTask Force()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Move/rename a branch and the corresponding reflog.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--move", "-m")]
        public GitBranchTask Move()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Copy a branch and the corresponding reflog.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--copy", "-c")]
        public GitBranchTask Copy()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// List or delete (if used with -d) the remote-tracking branches. Combine with --list to match the optional pattern(s).
        /// </summary>
        /// <returns></returns>
        [ArgKey("--remotes", "-r")]
        public GitBranchTask Remotes()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// List both remote-tracking branches and local branches. Combine with --list to match optional pattern(s).
        /// </summary>
        /// <returns></returns>
        [ArgKey("--all", "-a")]
        public GitBranchTask All()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// List branches.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--list", "-l")]
        public GitBranchTask List()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// When creating a new branch, set up branch.<name>.remote and branch.'name'.merge configuration entries to mark the start-point branch as "upstream" from the new branch.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--track", "-t")]
        public GitBranchTask Track()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Do not set up "upstream" configuration, even if the branch.autoSetupMerge configuration variable is true.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-track")]
        public GitBranchTask NoTrack()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Set up 'branchname''s tracking information so 'upstream' is considered 'branchname''s upstream branch. If no 'branchname' is specified, then it defaults to the current branch.
        /// </summary>
        /// <param name="upstream"></param>
        /// <returns></returns>
        [ArgKey("--set-upstream-to", "-u")]
        public GitBranchTask SetUpstreamTo(string upstream)
        {
            WithArgumentsKeyFromAttribute(upstream);
            return this;
        }

        /// <summary>
        /// Remove the upstream information for "branchname". If no branch is specified it defaults to the current branch.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--unset-upstream")]
        public GitBranchTask UnsetUpstream()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }
    }
}

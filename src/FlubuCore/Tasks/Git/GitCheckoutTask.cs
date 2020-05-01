using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Git
{
    public class GitCheckoutTask : ExternalProcessTaskBase<int, GitCheckoutTask>
    {
        public GitCheckoutTask(string branch = null)
        {
            ExecutablePath = "git";
            AddPrefixToAdditionalOptionKey(PrefixProcessors.AddDoubleDashPrefixToAdditionalOptionKey);
            WithArguments("checkout");
            if (!string.IsNullOrEmpty(branch))
            {
                WithArguments(branch);
            }

            AddAdditionalOptionPrefix("Checkout");
        }

        protected override string Description
        {
            get => "Executes git command 'checkout' with specified option.";
            set { }
        }

        /// <summary>
        /// Detach repo.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--detach")]
        public GitCheckoutTask Detach()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Create a new branch named <new_branch> and start it at <start_point>;
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [ArgKey("-b")]
        public GitCheckoutTask NewBranch(string name)
        {
            WithArgumentsKeyFromAttribute(name);
            return this;
        }

        /// <summary>
        /// Creates the branch <new_branch> and start it at <start_point>; if it already exists, then reset it to <start_point>.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [ArgKey("-B")]
        public GitCheckoutTask NewBranchWithReset(string name)
        {
            WithArgumentsKeyFromAttribute(name);
            return this;
        }

        /// <summary>
        /// When creating a new branch, set up "upstream" configuration.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--track", "-t")]
        public GitCheckoutTask Track()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Do not set up "upstream" configuration, even if the branch.autoSetupMerge configuration variable is true.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-track")]
        public GitCheckoutTask NoTrack()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Create a new orphan branch, named <new_branch>, started from <start_point> and switch to it.
        /// </summary>
        /// <returns></returns>
        [ArgKey("---orphan")]
        public GitCheckoutTask Orphan()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// When switching branches, if you have local modifications to one or more files that are different between the current branch and the branch to which you are switching, the command refuses to switch branches in order to preserve your modifications in context.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--merge")]
        public GitCheckoutTask Merge()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Using --recurse-submodules will update the content of all initialized submodules according to the commit recorded in the superproject.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--recurse-submodules")]
        public GitCheckoutTask RecurseSubmodules()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Interactively select hunks in the difference between the 'tree-ish' (or the index, if unspecified) and the working tree.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--patch", "-p")]
        public GitCheckoutTask Patch()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }
    }
}

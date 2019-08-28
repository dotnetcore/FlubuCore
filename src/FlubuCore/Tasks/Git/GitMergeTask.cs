using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Git
{
    public class GitMergeTask : ExternalProcessTaskBase<int, GitMergeTask>
    {
        private string _description;

        public GitMergeTask()
        {
            ExecutablePath = "git";
            InsertArgument(0, "merge");
            AddPrefixToAdditionalOptionKey(PrefixProcessors.AddDoubleDashPrefixToAdditionalOptionKey);
            AddAdditionalOptionPrefix("merge");
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes git command 'merge' with specified option.";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// Be verbose.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--verbose")]
        public GitMergeTask Verbose()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Perform the merge and commit the result. This option can be used to override --no-commit.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--commit")]
        public GitMergeTask Commit()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// With --no-commit perform the merge and stop just before creating a merge commit.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-commit")]
        public GitMergeTask NoCommit()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// When the merge resolves as a fast-forward, only update the branch pointer, without creating a merge commit.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--ff")]
        public GitMergeTask FastForward()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        [ArgKey("--cleanup")]
        public GitMergeTask Cleanup(string mode)
        {
            WithArgumentsKeyFromAttribute(mode);
            return this;
        }

        /// <summary>
        /// Create a merge commit even when the merge resolves as a fast-forward.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-ff")]
        public GitMergeTask NoFastForward()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Refuse to merge and exit with a non-zero status unless the current HEAD is already up to date or the merge can be resolved as a fast-forward.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--ff-only")]
        public GitMergeTask FastForwardOnly()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Show a diffstat at the end of the merge.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--stat")]
        public GitMergeTask Stat()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Produce the working tree and index state as if a real merge happened (except for the merge information), but do not actually make a commit, move the HEAD, or record.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--squash")]
        public GitMergeTask Squash()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Use the given merge strategy; can be supplied more than once to specify them in the order they should be tried.
        /// </summary>
        /// <param name="strategy"></param>
        /// <returns></returns>
        [ArgKey("--strategy")]
        public GitMergeTask Strategy(string strategy)
        {
            WithArgumentsKeyFromAttribute(strategy);
            return this;
        }

        /// <summary>
        /// Pass merge strategy specific option through to the merge strategy.
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        [ArgKey("--strategy-option")]
        public GitMergeTask StrategyOption(string option)
        {
            WithArgumentsKeyFromAttribute(option);
            return this;
        }

        /// <summary>
        /// Verify that the tip commit of the side branch being merged is signed with a valid key, i.e. a key that has a valid uid: in the default trust model, this means the signing key has been signed by a trusted key.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--verify-signatures")]
        public GitMergeTask VerifySignatures()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Turn progress on explicitly. If neither is specified, progress is shown if standard error is connected to a terminal.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--progress")]
        public GitMergeTask Progress()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Turn progress off explicitly. If neither is specified, progress is shown if standard error is connected to a terminal.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-progress")]
        public GitMergeTask NoProgress()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// By default, git merge command refuses to merge histories that do not share a common ancestor. This option can be used to override this safety when merging histories of two projects that started their lives independently.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--allow-unrelated-histories")]
        public GitMergeTask AllowUnrelatedHistories()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Set the commit message to be used for the merge commit (in case one is created).
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [ArgKey("-m")]
        public GitMergeTask Message(string message)
        {
            WithArgumentsKeyFromAttribute(message);
            return this;
        }

        /// <summary>
        /// Read the commit message to be used for the merge commit (in case one is created).
        /// </summary>
        /// <returns></returns>
        [ArgKey("--file", "-f")]
        public GitMergeTask File()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Allow the rerere mechanism to update the index with the result of auto-conflict resolution if possible.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--rerere-autoupdate")]
        public GitMergeTask RerereAutoupdate()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Silently overwrite ignored files from the merge result. This is the default behavior. Use --no-overwrite-ignore to abort.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-overwrite-ignore")]
        public GitMergeTask NoOverwriteIgnore()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Abort the current conflict resolution process, and try to reconstruct the pre-merge state.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--abort")]
        public GitMergeTask Abort()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Forget about the current merge in progress. Leave the index and the working tree as-is.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--quit")]
        public GitMergeTask Quit()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// After a git merge stops due to conflicts you can conclude the merge by running git merge --continue.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--continue")]
        public GitMergeTask Continue()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }
    }
}

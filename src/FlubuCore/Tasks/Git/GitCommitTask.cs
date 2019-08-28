using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Git
{
    public class GitCommitTask : ExternalProcessTaskBase<int, GitCommitTask>
    {
        private readonly List<string> _files;

        private string _description;

        public GitCommitTask()
         {
             _files = new List<string>();
             ExecutablePath = "git";
             InsertArgument(0, "commit");
             AddPrefixToAdditionalOptionKey(PrefixProcessors.AddDoubleDashPrefixToAdditionalOptionKey);
             AddAdditionalOptionPrefix("Commit");
         }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes git command 'commit' with specified option.";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// Override the commit author. Specify an explicit author using the standard A U Thor author@example.com format.
        /// </summary>
        /// <param name="author"></param>
        /// <returns></returns>
        [ArgKey("--author")]
        public GitCommitTask Author(string author)
        {
            WithArgumentsKeyFromAttribute(author);
            return this;
        }

        /// <summary>
        /// Use the given 'msg' as the commit message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [ArgKey("--message", "-m")]
        public GitCommitTask Message(string message)
        {
            WithArgumentsKeyFromAttribute(message);
            return this;
        }

        /// <summary>
        /// Tell the command to automatically stage files that have been modified and deleted, but new files you have not told Git about are not affected.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--all", "-a")]
        public GitCommitTask CommitAll()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Do not create a commit, but show a list of paths that are to be committed, paths with local changes that will be left uncommitted and paths that are untracked.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--dry-run")]
        public GitCommitTask DryRun()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// be more verbose.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--verbose")]
        public GitCommitTask Verbose()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// be more quiet.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--quiet")]
        public GitCommitTask Quiet()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Use the interactive patch selection interface to chose which changes to commit. See git-add[1] for details.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--patch", "-P")]
        public GitCommitTask Patch()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Take an existing commit object, and reuse the log message and the authorship information (including the timestamp) when creating the commit.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--reuse-message", "-C")]
        public GitCommitTask ReuseMessage(string commitId)
        {
            WithArgumentsKeyFromAttribute(commitId);
            return this;
        }

        /// <summary>
        /// Like --reuse-message, but with --reedit-message the editor is invoked, so that the user can further edit the commit message.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--reedit-message", "-c")]
        public GitCommitTask ReeditMessage(string commitId)
        {
            WithArgumentsKeyFromAttribute(commitId);
            return this;
        }

        /// <summary>
        /// Construct a commit message for use with rebase --autosquash. The commit message will be the subject line from the specified commit with a prefix of "fixup! ". See git-rebase[1] for details.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--fixup")]
        public GitCommitTask Fixup(string commitId)
        {
            WithArgumentsKeyFromAttribute(commitId);
            return this;
        }

        [ArgKey("--squash")]
        public GitCommitTask Squash(string commitId)
        {
            WithArgumentsKeyFromAttribute(commitId);
            return this;
        }

        /// <summary>
        /// Take the commit message from the given file. Use - to read the message from the standard input.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--file", "-F")]
        public GitCommitTask File(string file)
        {
            WithArgumentsKeyFromAttribute(file);
            return this;
        }

        /// <summary>
        /// Override the author date used in the commit.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        [ArgKey("--date")]
        public GitCommitTask Date(string dateTime)
        {
            WithArgumentsKeyFromAttribute(dateTime);
            return this;
        }

        /// <summary>
        /// When editing the commit message, start the editor with the contents in the given file. The commit.template configuration variable is often used to give this option implicitly to the command.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--template", "-t")]
        public GitCommitTask Template()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Add Signed-off-by line by the committer at the end of the commit log message.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--signoff", "-s")]
        public GitCommitTask SignOff()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// This option bypasses the pre-commit and commit-msg hooks. See also githooks[5].
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-verify", "-n")]
        public GitCommitTask NoVerify()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Usually recording a commit that has the exact same tree as its sole parent commit is a mistake, and the command prevents you from making such a commit.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--allow-empty")]
        public GitCommitTask AllowEmpty()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Like --allow-empty this command is primarily for use by foreign SCM interface scripts. It allows you to create a commit with an empty commit message without using plumbing commands
        /// </summary>
        /// <returns></returns>
        [ArgKey("--allow-empty-message")]
        public GitCommitTask AllowEmptyMessage()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// This option determines how the supplied commit message should be cleaned up before committing. The <mode> can be strip, whitespace, verbatim, scissors or default.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        [ArgKey("--cleanup")]
        public GitCommitTask Cleanup(string mode)
        {
            WithArgumentsKeyFromAttribute(mode);
            return this;
        }

        /// <summary>
        /// Use the selected commit message without launching an editor. For example, git commit --amend --no-edit amends a commit without changing its commit message.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-edit")]
        public GitCommitTask NoEdit()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Replace the tip of the current branch by creating a new commit.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--amend")]
        public GitCommitTask Amend()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Bypass the post-rewrite hook.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-post-rewrite")]
        public GitCommitTask NoPostRewrite()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Show untracked files.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--untracked-files")]
        public GitCommitTask UntrackedFiles()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// When files are given on the command line, the command commits the contents of the named files, without recording the changes already staged. The contents of these files are also staged for the next commit on top of what have been staged before.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public GitCommitTask AddFile(string file)
        {
            _files.Add(file);
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            if (_files.Count != 0)
            {
                WithArguments(_files.ToArray());
            }

            return base.DoExecute(context);
        }
    }
}

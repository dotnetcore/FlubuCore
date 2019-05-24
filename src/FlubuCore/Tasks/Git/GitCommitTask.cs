using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
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
        public GitCommitTask Author(string author)
        {
            WithArgumentsValueRequired("--author", author);
            return this;
        }

        /// <summary>
        /// Use the given as the commit message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public GitCommitTask Message(string message)
        {
            WithArgumentsValueRequired("--message", message);
            return this;
        }

        /// <summary>
        /// Tell the command to automatically stage files that have been modified and deleted, but new files you have not told Git about are not affected.
        /// </summary>
        /// <returns></returns>
        public GitCommitTask CommitAll()
        {
            WithArguments("--all");
            return this;
        }

        /// <summary>
        /// Do not create a commit, but show a list of paths that are to be committed, paths with local changes that will be left uncommitted and paths that are untracked.
        /// </summary>
        /// <returns></returns>
        public GitCommitTask DryRun()
        {
            WithArguments("--dry-run");
            return this;
        }

        /// <summary>
        /// be more verbose.
        /// </summary>
        /// <returns></returns>
        public GitCommitTask Verbose()
        {
            WithArguments("--verbose");
            return this;
        }

        /// <summary>
        /// be more quiet.
        /// </summary>
        /// <returns></returns>
        public GitCommitTask Quiet()
        {
            WithArguments("--quit");
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

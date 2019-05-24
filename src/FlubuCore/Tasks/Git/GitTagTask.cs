using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Git
{
    public class GitTagTask : ExternalProcessTaskBase<int, GitTagTask>
    {
        private readonly string _tagName;
        private string _description;

        /// <summary>
        /// Task updates the index using the current content found in the working tree, to prepare the content staged for the next commit.
        /// </summary>
        public GitTagTask(string tagName)
        {
            _tagName = tagName;
            ExecutablePath = "git";
            InsertArgument(0, "tag");
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes git command 'tag' with specified options.";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// Delete tag(s).
        /// </summary>
        /// <returns></returns>
        public GitTagTask Delete()
        {
            WithArguments("--delete");
            return this;
        }

        /// <summary>
        /// annotated tag, needs a message.
        /// </summary>
        /// <returns></returns>
        public GitTagTask Annotate()
        {
            WithArguments("--annotate");
            return this;
        }

        /// <summary>
        /// tag message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public GitTagTask Message(string message)
        {
            WithArgumentsValueRequired("--message", message);
            return this;
        }

        /// <summary>
        /// replace the tag if exists.
        /// </summary>
        /// <returns></returns>
        public GitTagTask Force()
        {
            WithArguments("--force");
            return this;
        }

        /// <summary>
        /// annotated and GPG-signed tag.
        /// </summary>
        /// <returns></returns>
        public GitTagTask GpgSign()
        {
            WithArguments("--sign");
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _tagName.MustNotBeNullOrEmpty("Tag name must not be null or empty.");
            WithArguments(_tagName);
            return base.DoExecute(context);
        }
    }
}

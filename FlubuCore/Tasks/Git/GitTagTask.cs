using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Git
{
    public class GitTagTask : ExternalProcessTaskBase<GitTagTask>
    {
        /// <summary>
        /// Task updates the index using the current content found in the working tree, to prepare the content staged for the next commit.
        /// </summary>
        public GitTagTask(string tagName)
        {
             InsertArgument(0, "tag");
             InsertArgument(1, tagName);
        }

        protected override string Description { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
    }
}

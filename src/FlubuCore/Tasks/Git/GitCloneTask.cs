using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Git
{
    public class GitCloneTask : ExternalProcessTaskBase<int, GitCloneTask>
    {
        private readonly string _repository;
        private readonly string _directory;
        private string _description;

        public GitCloneTask(string repository, string directory)
        {
            _repository = repository;
            _directory = directory;
            ExecutablePath = "git";
            InsertArgument(0, "clone");
            AddPrefixToAdditionalOptionKey(PrefixProcessors.AddDoubleDashPrefixToAdditionalOptionKey);
            AddAdditionalOptionPrefix("Clone");
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes git command 'clone' with specified option.";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// checkout specified branch instead of the remote's HEAD.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [ArgKey("--branch")]
        public GitCloneTask Branch(string name)
        {
            WithArgumentsKeyFromAttribute(name);
            return this;
        }

        /// <summary>
        /// force progress reporting.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--progress")]
        public GitCloneTask ShowProgress()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// be more verbose.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--verbose", "-v")]
        public GitCloneTask Verbose()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// be more quiet.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--quiet", "-q")]
        public GitCloneTask Quiet()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// don't create a checkout
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-checkout")]
        public GitCloneTask NoCheckout()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// create a bare repository.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--bare")]
        public GitCloneTask CreateBareRepository()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// setup as shared repository.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--shared")]
        public GitCloneTask SetupAsSharedRepository()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _repository.MustNotBeNullOrEmpty("Url of the repository to clone must not be empty.");
            WithArguments(_repository, _directory);
            return base.DoExecute(context);
        }
    }
}
